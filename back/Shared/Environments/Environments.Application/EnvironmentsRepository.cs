using Environments.Domain;
using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tools;

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

        public async Task<Page<Environment>> GetAsync(IPageToken page, EnvironmentFilter filter)
        {
            var rights = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            return await _store.GetAsync(page, rights, filter);
        }

        public async Task<HashSet<DistributorWithAccess>> GetAccessesAsync(string subdomain)
        {
            var rights = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            var envs = await _store.GetAsync(rights, new EnvironmentFilter
            {
                Subdomain = CompareString.Equals(subdomain),
                IsActive = CompareBoolean.TrueOnly
            });

            var env = envs.FirstOrDefault();

            if (env is null)
            {
                throw new NotFoundException($"No active environment with subdomain {subdomain} was found");
            }

            return env.ActiveAccesses
                .GroupBy(a => a.Consumer.Code)
                .Select(group => new DistributorWithAccess(group.Key, group.Select(i => i.Access.Type).ToHashSet()))
                .ToHashSet();
        }
    }
}
