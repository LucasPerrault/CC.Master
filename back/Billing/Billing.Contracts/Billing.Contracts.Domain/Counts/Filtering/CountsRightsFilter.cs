using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Rights.Domain.Abstractions;

namespace Billing.Contracts.Domain.Contracts
{
    public class CountsRightsFilter
    {
        private readonly IRightsService _rightsService;
        private readonly RightsFilter _rightsFilter;

        public CountsRightsFilter(IRightsService rightsService, RightsFilter rightsFilter)
        {
            _rightsService = rightsService;
            _rightsFilter = rightsFilter;
        }

        public Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            return GetAccessAsync(principal, Operation.ReadCounts);
        }

        private async Task<AccessRight> GetAccessAsync(ClaimsPrincipal principal, Operation op)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal user => await _rightsFilter.FilterByDistributorAsync(Operation.ReadContracts, user.User.Distributor.Id),
                CloudControlApiKeyClaimsPrincipal _ => await GetApiKeyReadAccessAsync(op),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }
        private async Task<AccessRight> GetApiKeyReadAccessAsync(Operation op)
        {
            var hasOp = await _rightsService.HasOperationAsync(op);
            return hasOp ? AccessRight.All : AccessRight.None;
        }
    }
}
