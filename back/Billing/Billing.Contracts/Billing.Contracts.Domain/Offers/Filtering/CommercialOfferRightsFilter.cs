using Authentication.Domain;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public class CommercialOfferRightsFilter
    {
        private readonly RightsFilter _rightsFilter;

        public CommercialOfferRightsFilter(RightsFilter rightsFilter)
        {
            _rightsFilter = rightsFilter;
        }

        public Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            var right = principal switch
            {
                CloudControlUserClaimsPrincipal _ => AccessRight.All,
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.All,
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
            return Task.FromResult(right);
        }
    }
}
