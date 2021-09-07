using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Domain;
using Rights.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Stores
{
    public interface ICloudControlPermissionsStore
    {
        Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations);
        Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId);
        Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId);
        Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations);
    }

    public class CloudControlPermissionsStore : ICloudControlPermissionsStore
    {

        private readonly ApiKeyPermissionsService _apiKeyPermissionsService;
        private readonly UserPermissionsService _userPermissionsService;

        public CloudControlPermissionsStore(ApiKeyPermissionsService apiKeyPermissionsService, UserPermissionsService userPermissionsService)
        {
            _apiKeyPermissionsService = apiKeyPermissionsService;
            _userPermissionsService = userPermissionsService;
        }

        public async Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
        {
            return new List<IApiKeyPermission>(await _apiKeyPermissionsService.GetApiKeyPermissionsAsync(apiKeyId, operations));
        }

        public Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId)
        {
            var operations = OperationHelper.GetAll().Select(o => (int)o).ToHashSet();
            return GetApiKeyPermissionsAsync(apiKeyId, operations);
        }

        public async Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            return new List<IUserPermission>(await _userPermissionsService.GetUserPermissionsAsync(principalId));
        }

        public async Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations)
        {
            return new List<IUserPermission>(await _userPermissionsService.GetUserPermissionsAsync(principalId, operations));
        }
    }
}
