using Authentication.Domain;
using Lucca.Core.Rights.Abstractions.Permissions;
using Remote.Infra.Services;
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
    public class ApiKeyPermissionsRemoteService
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public ApiKeyPermissionsRemoteService(HttpClient httpClient, ClaimsPrincipal claimsPrincipal)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient,"Partenaires api keys");
            _claimsPrincipal = claimsPrincipal;
        }

        internal async Task<IReadOnlyCollection<IApiKeyPermission>> GetApiKeyPermissionsAsync()
        {
            if (!(_claimsPrincipal is CloudControlApiKeyClaimsPrincipal))
            {
                throw new ApplicationException("Method assumes ClaimsPrincipal is an api key, but it is not");
            }

            var queryParams = new Dictionary<string, string>
            {
                { "appInstanceId", RightsHelper.CloudControlAppInstanceId.ToString() },
                { "fields", ApiKeyPermission.ApiFields }
            };

            var apiKeyPermissionsResponse = await _httpClientHelper.GetObjectCollectionResponseAsync<ApiKeyPermission>(queryParams);

            var allApiKeyPermissions = apiKeyPermissionsResponse.Data.Items;

            return allApiKeyPermissions.ToList();
        }
    }
}
