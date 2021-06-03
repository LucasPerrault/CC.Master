using Partenaires.Infra.Services;
using Rights.Infra.Models;
using Rights.Infra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class UserPermissionsRemoteService : PartenairesService
    {
        protected override string RemoteApiDescription => "Partenaires users permissions";
        public UserPermissionsRemoteService(HttpClient httpClient) : base(httpClient)
        { }

        internal async Task<IReadOnlyCollection<Permission>> GetUserPermissionsAsync(int principalId)
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

            var userPermissions = await GetObjectCollectionResponseAsync<Permission>(queryParams);
            return userPermissions.Data.Items;
        }
    }
}
