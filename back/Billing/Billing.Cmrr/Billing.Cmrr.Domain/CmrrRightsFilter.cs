using Authentication.Domain;
using Distributors.Domain;
using Rights.Domain;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Cmrr.Domain
{
    public class CmrrRightsFilter : ICmrrRightsFilter
    {
        private readonly IDistributorsStore _distributorStore;
        private readonly RightsFilter _rightsFilter;

        public CmrrRightsFilter(IDistributorsStore distributorStore, RightsFilter rightsFilter)
        {
            _distributorStore = distributorStore;
            _rightsFilter = rightsFilter;
        }

        public async Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {

            return principal switch
            {
                CloudControlUserClaimsPrincipal userPrincipal => await _rightsFilter.FilterByDistributorAsync(Operation.ReadContracts, userPrincipal.User.DistributorCode),
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.All,
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
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
