using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferRightsFilter
    {
        private readonly IRightsService _rightsService;

        public CommercialOfferRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            return GetAccessAsync(principal, Operation.ReadCommercialOffers);
        }

        public Task<AccessRight> GetWriteAccessAsync(ClaimsPrincipal principal)
        {
            return GetAccessAsync(principal, Operation.CreateCommercialOffers);
        }

        private async Task<AccessRight> GetAccessAsync(ClaimsPrincipal principal, Operation op)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal _ => await GetUserReadAccessAsync(op),
                CloudControlApiKeyClaimsPrincipal _ => await GetApiKeyReadAccessAsync(op),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }

        private async Task<AccessRight> GetUserReadAccessAsync(Operation op)
        {
            var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(op);
            return currentUserScope switch
            {
                AccessRightScope.AllDistributors => AccessRight.All,
                _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
            };
        }

        private async Task<AccessRight> GetApiKeyReadAccessAsync(Operation op)
        {
            var hasOp = await _rightsService.HasOperationAsync(op);
            return hasOp ? AccessRight.All : AccessRight.None;
        }
    }
}
