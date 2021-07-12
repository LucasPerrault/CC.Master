using Authentication.Domain;
using Distributors.Domain;
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
        private readonly IDistributorsStore _distributorStore;

        public CmrrRightsFilter(IRightsService rightsService, IDistributorsStore distributorStore)
        {
            _rightsService = rightsService;
            _distributorStore = distributorStore;
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
                        AccessRightScope.OwnDistributorOnly => await GetDistributorIdAccessRightFromDistributorCodeAsync(userPrincipal.User.DistributorCode),
                        _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
                    };
                case CloudControlApiKeyClaimsPrincipal _:
                    return AccessRight.All;
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            };
        }

        private async Task<AccessRight> GetDistributorIdAccessRightFromDistributorCodeAsync(string distributorCode)
        {
            var distributor = await _distributorStore.GetByCodeAsync(distributorCode);
            return AccessRight.ForDistributorId(distributor.Id);
        }
    }

    public interface ICmrrRightsFilter
    {
        Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal);
    }
}
