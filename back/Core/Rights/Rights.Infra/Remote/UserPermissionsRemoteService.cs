using Lucca.Core.Rights.Abstractions.Permissions;
using Remote.Infra.Services;
using Rights.Infra.Models;
using Rights.Infra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class UserPermissionsRemoteService
    {
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public UserPermissionsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires users permissions");
        }

        internal async Task<IReadOnlyCollection<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            var allPermissions = await GetAllUserPermissionsAsync(principalId);
            return allPermissions.ToList();
        }

        private async Task<IEnumerable<Permission>> GetAllUserPermissionsAsync(int principalId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "appInstanceId", RightsHelper.CloudControlAppInstanceId.ToString() },
                { "userId", principalId.ToString() },
                { "fields", Permission.ApiFields }
            };

            var userPermissions = await _httpClientHelper.GetObjectCollectionResponseAsync<Permission>(queryParams);
            return userPermissions.Data.Items;
        }
    }
}
