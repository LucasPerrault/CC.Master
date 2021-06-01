using Authentication.Domain;
using Environments.Domain.Storage;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<EnvironmentAccessRight>> GetAccessRightAsync(ClaimsPrincipal principal, Operation operation)
        {
            switch (principal)
            {
                case CloudControlApiKeyClaimsPrincipal _:
                {
                    var purposes = await _rightsService.GetEnvironmentPurposesAsync(operation);
                    return new List<EnvironmentAccessRight>
                    {
                        new EnvironmentAccessRight(AccessRight.All, PurposeAccessRight.ForSome(purposes))
                    };
                }
                case CloudControlUserClaimsPrincipal user:
                {
                    var permissions = await _rightsService.GetScopedPermissionsAsync(operation);
                    return permissions.Select
                    (
                        p => new EnvironmentAccessRight
                        (
                            p.Scope switch
                            {
                                AccessRightScope.AllDistributors => AccessRight.All,
                                AccessRightScope.OwnDistributorOnly => AccessRight.ForDistributor(user.User.DepartmentCode)
                            },
                            PurposeAccessRight.ForSome(p.EnvironmentPurposes)
                        )
                        ).ToList();
                }
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            }
        }
    }
}
