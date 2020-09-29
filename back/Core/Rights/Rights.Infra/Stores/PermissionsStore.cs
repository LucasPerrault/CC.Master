using Lucca.Core.Rights.Abstractions.Permissions;
using Lucca.Core.Rights.Abstractions.Stores;
using Rights.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rights.Infra.Stores
{
	public class PermissionsStore : IPermissionsStore
	{
		private readonly ApiKeyPermissionsService _apiKeyPermissionsService;
		private readonly UserPermissionsService _userPermissionsService;

		public PermissionsStore(ApiKeyPermissionsService apiKeyPermissionsService, UserPermissionsService userPermissionsService)
		{
			_apiKeyPermissionsService = apiKeyPermissionsService;
			_userPermissionsService = userPermissionsService;
		}

		public async Task<List<IApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, int appInstanceId, ISet<int> operations)
		{
			return new List<IApiKeyPermission>(await _apiKeyPermissionsService.GetApiKeyPermissionsAsync(apiKeyId, operations));
		}

		public async Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, int appInstanceId)
		{
			return new List<IUserPermission>(await _userPermissionsService.GetUserPermissionsAsync(principalId));
		}

		public async Task<List<IUserPermission>> GetUserPermissionsAsync(int principalId, int appInstanceId, ISet<int> operations)
		{
			return new List<IUserPermission>(await _userPermissionsService.GetUserPermissionsAsync(principalId, operations));
		}

		public Task<List<IWebServicePermission>> GetWebServicesPermissionsAsync(string webServiceId, int appInstanceId, ISet<int> operations)
		{
			return Task.FromResult(new List<IWebServicePermission>());
		}
	}
}
