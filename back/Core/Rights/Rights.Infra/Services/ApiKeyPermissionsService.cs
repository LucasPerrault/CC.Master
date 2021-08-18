using Cache.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class ApiKeyPermissionsCache : InMemoryCache<int, List<IApiKeyPermission>>
    {
        public ApiKeyPermissionsCache() : base(TimeSpan.FromSeconds(30))
        { }
    }

    public class ApiKeyPermissionsService
    {
        private readonly ApiKeyPermissionsRemoteService _remoteService;
        private readonly ApiKeyPermissionsCache _permissionsCache;

        public ApiKeyPermissionsService(ApiKeyPermissionsRemoteService remoteService, ApiKeyPermissionsCache permissionsCache)
        {
            _remoteService = remoteService;
            _permissionsCache = permissionsCache;
        }

        internal async Task<IReadOnlyCollection<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
        {
            if (_permissionsCache.TryGet(apiKeyId, out var cachedPermissions))
            {
                return cachedPermissions;
            }

            var permissions = (await _remoteService.GetApiKeyPermissionsAsync(apiKeyId))
                .ToList();
            _permissionsCache.Cache(apiKeyId, permissions);
            return permissions;
        }
    }
}
