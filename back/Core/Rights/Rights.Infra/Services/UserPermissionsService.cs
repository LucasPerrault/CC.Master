using Lucca.Core.Rights.Abstractions.Permissions;
using Rights.Infra.Models;
using Rights.Infra.Remote;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
	public class UserPermissionsService
    {
		private readonly UserPermissionsRemoteService _remoteService;
		private List<Permission> _cache;

		public UserPermissionsService(UserPermissionsRemoteService remoteService)
		{
			_remoteService = remoteService;
		}

		internal async Task<IReadOnlyCollection<Permission>> GetUserPermissionsAsync(int principalId)
		{
			if (_cache == null)
			{
				_cache = (await _remoteService.GetUserPermissionsAsync(principalId)).ToList();
			}
			return _cache;
		}

		internal async Task<IEnumerable<IUserPermission>> GetUserPermissionsAsync(int principalId, ISet<int> operations)
		{
			return (await GetUserPermissionsAsync(principalId)).Where(p => operations.Contains(p.OperationId)).ToList();
		}
	}
}
