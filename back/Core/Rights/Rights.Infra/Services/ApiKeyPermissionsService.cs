using Rights.Infra.Models;
using Rights.Infra.Remote;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Domain.Services
{
	public class ApiKeyPermissionsService
	{
		private readonly ApiKeyPermissionsRemoteService _remoteService;
		private List<ApiKeyPermission> _cache;

		public ApiKeyPermissionsService(ApiKeyPermissionsRemoteService remoteService)
		{
			_remoteService = remoteService;
		}

		internal async Task<IReadOnlyCollection<ApiKeyPermission>> GetApiKeyPermissionsAsync(int apiKeyId, ISet<int> operations)
		{
			if (_cache == null)
			{
				_cache = (await _remoteService.GetApiKeyPermissionsAsync(apiKeyId)).ToList();
			}
			return _cache.Where(p => operations.Contains(p.OperationId)).ToList();
		}
	}
}
