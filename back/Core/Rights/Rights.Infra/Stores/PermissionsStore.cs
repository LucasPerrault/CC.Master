using Lucca.Core.Rights.Abstractions.Permissions;
using Lucca.Core.Rights.Abstractions.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rights.Infra.Stores
{
    public class PermissionsStore : IPermissionsStore
    {
        private readonly ICloudControlPermissionsStore _permissionsStore;

        public PermissionsStore(ICloudControlPermissionsStore permissionsStore)
        {
            _permissionsStore = permissionsStore;
        }

        public Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, int appInstanceId, ISet<int> operations)
        {
            return _permissionsStore.GetApiKeyPermissionsAsync(apiKeyId, operations);
        }

        public Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, int appInstanceId)
        {
            return _permissionsStore.GetUserPermissionsAsync(principalId);
        }

        public Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, int appInstanceId, ISet<int> operations)
        {
            return _permissionsStore.GetUserPermissionsAsync(principalId, operations);
        }

        public Task<List<IWebServicePermission>> GetWebServicesPermissionsAsync(string webServiceId, int appInstanceId, ISet<int> operations)
        {
            return Task.FromResult(new List<IWebServicePermission>());
        }
    }
}
