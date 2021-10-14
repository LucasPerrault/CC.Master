using Environments.Domain;
using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Page<Environment>> GetAsync(IPageToken page, EnvironmentFilter filter)
        {
            var rights = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            return await _store.GetAsync(page, rights, filter);
        }

        public async Task<Page<EnvironmentWithAccess>> GetAccessesAsync(IPageToken queryPage, EnvironmentFilter filter)
        {
            var rights = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            var envs = await _store.GetAsync(queryPage, rights, filter);
            var accesses = envs.Items.Select(ToEnvironmentWithAccess).ToList();
            return new Page<EnvironmentWithAccess>
            {
                Items = accesses,
                Count = envs.Count,
                Next = envs.Next,
                Prev = envs.Prev
            };
        }

        private static EnvironmentWithAccess ToEnvironmentWithAccess(Environment environment)
        {
            return new EnvironmentWithAccess
            {
                Id = environment.Id,
                Subdomain = environment.Subdomain,
                Accesses = environment.ActiveAccesses
                    .GroupBy(a => a.Consumer.Code)
                    .Select(group => new DistributorWithAccess(group.Select(a => a.Consumer).First(), group.Select(i => i.Access.Type).ToHashSet()))
                    .ToHashSet()
            };
        }
    }
}
