using Cache.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class UserPermissionsCache : InMemoryCache<int, List<IUserPermission>>
    {
        public UserPermissionsCache() : base(TimeSpan.FromSeconds(30))
        { }
    }

    public class UserPermissionsService
    {
        private readonly UserPermissionsRemoteService _remoteService;
        private readonly UserPermissionsCache _userPermissionsCache;

        public UserPermissionsService(UserPermissionsRemoteService remoteService, UserPermissionsCache userPermissionsCache)
        {
            _remoteService = remoteService;
            _userPermissionsCache = userPermissionsCache;
        }

        internal async Task<IReadOnlyCollection<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            if (_userPermissionsCache.TryGet(principalId, out var cachedPermissions))
            {
                return cachedPermissions;
            }

            var permissions = (await _remoteService.GetUserPermissionsAsync(principalId))
                .ToList();
            _userPermissionsCache.Cache(principalId, permissions);

            return permissions;
        }

        internal async Task<IEnumerable<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations)
        {
            return (await GetUserPermissionsAsync(principalId)).Where(p => operations.Contains(p.OperationId)).ToList();
        }
    }
}
