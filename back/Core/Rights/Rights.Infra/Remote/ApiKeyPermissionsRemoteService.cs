using Authentication.Domain;
using Newtonsoft.Json;
using Partenaires.Infra.Services;
using Rights.Infra.Models;
using Rights.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class ApiKeyPermissionsRemoteService : PartenairesService
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        protected override string RemoteApiDescription => "Partenaires api keys";

        public ApiKeyPermissionsRemoteService
            (HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
            : base(httpClient, jsonSerializer)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        internal async Task<IReadOnlyCollection<ApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId)
        {
            // apiKeyId is not known (Lucca apis hide it by design)
            // calling as current api key principal
            // remote instance will filter results accordingly
            // https://github.com/LuccaSA/ilucca/blob/e5a6707565918cfb09388717344a60ce9e1321e6/Domain/Lucca.Domain/Repositories/ForeignAppPermissionsLookupRepository.cs#L40
            if (!(_claimsPrincipal is CloudControlApiKeyClaimsPrincipal))
            {
                throw new ApplicationException("Method assumes ClaimsPrincipal is an api key, but it is not");
            }

            var queryParams = new Dictionary<string, string>
            {
                { "appInstanceId", RightsHelper.CloudControlAppInstanceId.ToString() },
                { "fields", ApiKeyPermission.ApiFields }
            };

            var apiKeyPermissionsResponse = await GetObjectCollectionResponseAsync<ApiKeyPermission>(queryParams);

            var allApiKeyPermissions = apiKeyPermissionsResponse.Data.Items;

            return allApiKeyPermissions.ToList();
        }
    }
}
