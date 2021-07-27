using Authentication.Domain;
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
        public async Task<AccessRight> GetAccessAsync(ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.None,
                CloudControlUserClaimsPrincipal user => GetAccessRight(user.User),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }

        private AccessRight GetAccessRight(User user)
        {
            return user.DistributorId == DistributorIds.Lucca
                ? AccessRight.All
                : AccessRight.ForDistributor(user.DistributorId);
        }
    }
}
