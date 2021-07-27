using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients
{
    public class ClientRightFilter
    {
        private readonly RightsFilter _rightsFilter;

        public ClientRightFilter(RightsFilter rightsFilter)
        {
            _rightsFilter = rightsFilter;
        }

        public async Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            return principal switch
            {
                CloudControlUserClaimsPrincipal userPrincipal => await _rightsFilter.FilterByDistributorAsync(Operation.ReadContracts, userPrincipal.User.DistributorId),
                CloudControlApiKeyClaimsPrincipal _ => AccessRight.All,
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }

        public AccessRight GetAccessForEnvironment(string environmentSubdomain)
        {
            return new EnvironmentAccessRight(environmentSubdomain);
        }
    }

    public class EnvironmentAccessRight : AccessRight
    {
        public string Subdomain { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Subdomain;
            }
        }

        internal EnvironmentAccessRight(string subdomain)
        {
            Subdomain = subdomain;
        }
    }
}
