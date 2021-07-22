using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Rights.Domain.Filtering;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Infra.Services
{
    public class ClientVisibilityService : IClientVisibilityService
    {
        private readonly IContractsStore _contractsStore;

        public ClientVisibilityService(IContractsStore contractsStore)
        {
            _contractsStore = contractsStore ?? throw new ArgumentNullException(nameof(contractsStore));
        }

        public async Task<bool> IsClientVisibleInSubdomainAsync(Guid clientExternalId, string subdomain)
        {
            var filter = new ContractFilter
            {
                Subdomain = CompareString.Equals(subdomain),
                ClientExternalId = clientExternalId
            };

            var contracts = await _contractsStore.GetAsync(AccessRight.All, filter);
            return contracts.Any();
        }
    }
}
