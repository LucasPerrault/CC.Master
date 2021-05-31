using Authentication.Domain;
using Environments.Domain.Storage;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Environments.Domain
{
    public class EnvironmentRightsFilter
    {
        private readonly IRightsService _rightsService;

        public EnvironmentRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public AccessRight GetAccessRight(ClaimsPrincipal principal)
        {
            return principal is CloudControlUserClaimsPrincipal userClaimsPrincipal
                ? AccessRight.ForDistributor(userClaimsPrincipal.User.DepartmentCode)
                : AccessRight.All;
        }

        public async Task<PurposeAccessRight> GetPurposesAccessRightAsync(Operation operation)
        {
            var purposes = await _rightsService.GetEnvironmentPurposesAsync(operation);
            return PurposeAccessRight.ForSome(purposes);
        }
    }
}
