using Cache.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Models;
using Rights.Infra.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{

    public class ApiKeyPermissionsCacheKey : CacheKey<byte[]>
    {
        private int UserId { get; }

        public ApiKeyPermissionsCacheKey(int userId)
        {
            UserId = userId;
        }

        public override string Key => $"api-key-permissions:{UserId}";
    }
    public class ApiKeyPermissionsService
    {
        private readonly ApiKeyPermissionsRemoteService _remoteService;
        private readonly ICacheService _cacheService;

        public ApiKeyPermissionsService(ApiKeyPermissionsRemoteService remoteService, ICacheService cacheService)
        {
            _remoteService = remoteService;
            _cacheService = cacheService;
        }

        internal async Task<IReadOnlyCollection<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
        {
            var key = new ApiKeyPermissionsCacheKey(apiKeyId);
            var cachedPermissionBytes = await _cacheService.GetAsync(key);
            if (cachedPermissionBytes != null)
            {
                return cachedPermissionBytes.ToApiKeyPermissions();
            }

            var permissions = (await _remoteService.GetApiKeyPermissionsAsync(apiKeyId))
                .ToList();
            await _cacheService.SetAsync(key, permissions.ToBytes(), CacheInvalidation.After(TimeSpan.FromMinutes(2)));
            return permissions;
        }
    }
}
