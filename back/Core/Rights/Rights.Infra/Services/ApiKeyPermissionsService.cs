using Authentication.Domain;
using Cache.Abstractions;
using Lock;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Models;
using Rights.Infra.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class ApiKeyPermissionsCacheKey : CacheKey<byte[]>
    {
        private Guid ApiKeyGuid { get; }

        public ApiKeyPermissionsCacheKey(Guid guid)
        {
            ApiKeyGuid = guid;
        }

        public override string Key => $"api-key-permissions:{ApiKeyGuid}";
    }

    public class ApiKeyPermissionsService
    {
        private readonly ApiKeyPermissionsRemoteService _remoteService;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly ClaimsPrincipal _principal;

        public ApiKeyPermissionsService(ApiKeyPermissionsRemoteService remoteService, ICacheService cacheService, ILockService lockService, ClaimsPrincipal principal)
        {
            _remoteService = remoteService;
            _cacheService = cacheService;
            _lockService = lockService;
            _principal = principal;
        }

        // apiKeyId is not known (Lucca apis hide it by design)
        // ApiKeyPermissionsRemoteService will make the call as current api key principal
        // remote instance will filter results accordingly
        // https://github.com/LuccaSA/ilucca/blob/e5a6707565918cfb09388717344a60ce9e1321e6/Domain/Lucca.Domain/Repositories/ForeignAppPermissionsLookupRepository.cs#L40
        internal async Task<IReadOnlyCollection<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
        {
            if (!( _principal is CloudControlApiKeyClaimsPrincipal apiKey ))
            {
                throw new ApplicationException("Method assumes ClaimsPrincipal is an api key, but it is not");
            }

            var key = new ApiKeyPermissionsCacheKey(apiKey.ApiKey.Token);

            var cachedPermissionBytes = await _cacheService.GetAsync(key);
            if (cachedPermissionBytes != null)
            {
                return cachedPermissionBytes.ToApiKeyPermissions();
            }

            using var acquiredLock = await _lockService.TakeLockAsync(key.Key, TimeSpan.FromSeconds(10));

            var permissionCachedByConcurrentProcess = await _cacheService.GetAsync(key);
            if (permissionCachedByConcurrentProcess != null)
            {
                return permissionCachedByConcurrentProcess.ToApiKeyPermissions();
            }

            var permissions = (await _remoteService.GetApiKeyPermissionsAsync()).ToList();
            await _cacheService.SetAsync(key, permissions.ToBytes(), CacheInvalidation.After(TimeSpan.FromMinutes(2)));
            return permissions;
        }
    }
}
