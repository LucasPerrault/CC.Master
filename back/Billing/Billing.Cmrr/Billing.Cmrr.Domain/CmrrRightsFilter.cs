using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Cmrr.Domain
{
    public class CmrrRightsFilter : ICmrrRightsFilter
    {
        private readonly IRightsService _rightsService;

        public CmrrRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public async Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            switch (principal)
            {
                case CloudControlUserClaimsPrincipal userPrincipal:
                    var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.ReadContracts);
                    return currentUserScope switch
                    {
                        AccessRightScope.AllDistributors => AccessRight.All,
                        AccessRightScope.OwnDistributorOnly => AccessRight.ForDistributor(userPrincipal.User.DistributorId),
                        _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
                    };
                case CloudControlApiKeyClaimsPrincipal _:
                    return AccessRight.All;
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            }
        }
    }

    public interface ICmrrRightsFilter
    {
        Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal);
    }
}
