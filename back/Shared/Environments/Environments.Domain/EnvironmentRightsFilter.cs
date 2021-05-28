using Authentication.Domain;
using Environments.Domain.Storage;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Domain;

namespace Environments.Domain
{
    public class EnvironmentRightsFilter
    {
        private readonly IRightsService _rightsService;

        public EnvironmentRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public async Task<EnvironmentAccessRight> GetAccessRightAsync(ClaimsPrincipal principal, Operation operation)
        {
            var purposes = await _rightsService.GetEnvironmentPurposesAsync(operation);
            var access = principal is CloudControlUserClaimsPrincipal userClaimsPrincipal
                ? AccessRight.ForDistributor(userClaimsPrincipal.User.DepartmentCode)
                : AccessRight.All;

            return new EnvironmentAccessRight
            {
                Purposes = PurposeAccessRight.ForSome(purposes),
                AccessRight = access
            };
        }
    }
}
