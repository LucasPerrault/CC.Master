using Cache.Abstractions;
using Lock;
using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Models;
using Rights.Infra.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class PermissionsCacheKey : CacheKey<byte[]>
    {
        private int UserId { get; }

        public PermissionsCacheKey(int userId)
        {
            UserId = userId;
        }

        public override string Key => $"permissions:{UserId}";
    }

    public class UserPermissionsService
    {
        private readonly UserPermissionsRemoteService _remoteService;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;

        public UserPermissionsService(UserPermissionsRemoteService remoteService, ICacheService cacheService, ILockService lockService)
        {
            _remoteService = remoteService;
            _cacheService = cacheService;
            _lockService = lockService;
        }

        internal async Task<IReadOnlyCollection<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            var key = new PermissionsCacheKey(principalId);

            var cachedPermissionBytes = await _cacheService.GetAsync(key);
            if (cachedPermissionBytes != null)
            {
                return cachedPermissionBytes.ToPermissions();
            }

            using var acquiredLock = await _lockService.TakeLockAsync(key.Key, TimeSpan.FromSeconds(10));

            var permissionCachedByConcurrentProcess = await _cacheService.GetAsync(key);
            if (permissionCachedByConcurrentProcess != null)
            {
                return permissionCachedByConcurrentProcess.ToPermissions();
            }

            var permissions = (await _remoteService.GetUserPermissionsAsync(principalId)).ToList();
            await _cacheService.SetAsync(key, permissions.ToBytes(), CacheInvalidation.After(TimeSpan.FromSeconds(30)));

            return permissions;
        }

        internal async Task<IEnumerable<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations)
        {
            return (await GetUserPermissionsAsync(principalId)).Where(p => operations.Contains(p.OperationId)).ToList();
        }
    }
}
