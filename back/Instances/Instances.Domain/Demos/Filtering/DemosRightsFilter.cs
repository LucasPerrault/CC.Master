using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoRightsFilter
    {
        private readonly RightsFilter _rightsFilter;

        public DemoRightsFilter(RightsFilter rightsFilter)
        {
            _rightsFilter = rightsFilter;
        }

        public async Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal userPrincipal => await _rightsFilter.FilterByDistributorAsync(Operation.Demo, userPrincipal.User.DistributorId),
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.All,
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }
    }
}
