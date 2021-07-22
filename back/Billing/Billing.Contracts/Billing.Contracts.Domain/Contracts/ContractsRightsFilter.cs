using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractsRightsFilter
    {
        private readonly RightsFilter _rightsFilter;

        public ContractsRightsFilter(RightsFilter rightsFilter)
        {
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
    }
}
