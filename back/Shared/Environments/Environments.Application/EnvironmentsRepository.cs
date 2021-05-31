using Environments.Domain;
using Environments.Domain.Storage;
using Rights.Domain;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Environments.Application
{
    public class EnvironmentsRepository
    {
        private readonly IEnvironmentsStore _store;
        private readonly ClaimsPrincipal _principal;
        private readonly EnvironmentRightsFilter _rightsFilter;

        public EnvironmentsRepository(IEnvironmentsStore store, ClaimsPrincipal principal, EnvironmentRightsFilter rightsFilter)
        {
            _store = store;
            _principal = principal;
            _rightsFilter = rightsFilter;
        }

        public async Task<List<Environment>> GetAsync()
        {
            var rights = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            return await _store.GetAsync(rights, new EnvironmentFilter());
        }
    }
}
