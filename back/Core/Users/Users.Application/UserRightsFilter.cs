using Authentication.Domain;
using Distributors.Domain;
using Distributors.Domain.Models;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Application
{
    // This could belong in Users.Domain,
    // but it would result in a dependency loop between Authentication.Domain and Users.Domain
    public class UserRightsFilter
    {
        private readonly IDistributorsStore _distributorsStore;

        public UserRightsFilter(IDistributorsStore distributorsStore)
        {
            _distributorsStore = distributorsStore;
        }

        public async Task<AccessRight> GetAccessAsync(ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.None,
                CloudControlUserClaimsPrincipal user => await GetAccessAsync(user.User),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }

        private async Task<AccessRight> GetAccessAsync(User user)
        {
            var distributor = await _distributorsStore.GetByIdAsync(user.DistributorId);
            return distributor.Id == DistributorIds.Lucca
                ? AccessRight.All
                : AccessRight.ForDistributor(user.DistributorId);
        }
    }
}
