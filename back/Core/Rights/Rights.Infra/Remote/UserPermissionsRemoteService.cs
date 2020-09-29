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
	public class UserPermissionsRemoteService : PartenairesService
	{
		public UserPermissionsRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
			: base(httpClient, jsonSerializer, claimsPrincipal)
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
