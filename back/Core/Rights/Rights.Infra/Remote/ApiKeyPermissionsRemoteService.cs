using Newtonsoft.Json;
using Partenaires.Infra.Services;
using Rights.Infra.Models;
using Rights.Infra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
	public class ApiKeyPermissionsRemoteService : PartenairesService
	{
		public ApiKeyPermissionsRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
			: base(httpClient, jsonSerializer, claimsPrincipal)
		{ }

		internal async Task<IReadOnlyCollection<ApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId)
		{
			var queryParams = new Dictionary<string, string>
			{
				{ "appInstanceId", RightsHelper.CloudControlAppInstanceId.ToString() },
				{ "foreignAppId", apiKeyId.ToString() },
				{ "fields", ApiKeyPermission.ApiFields }
			};

			var apiKeyPermissionsReponse = await GetObjectCollectionResponseAsync<ApiKeyPermission>(queryParams);

			var allApiKeyPermissions = apiKeyPermissionsReponse.Data.Items;

			return allApiKeyPermissions.ToList();
		}
	}
}
