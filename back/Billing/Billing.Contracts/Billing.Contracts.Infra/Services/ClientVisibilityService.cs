using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Contracts.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var contracts = await _contractsStore.GetAsync(clientExternalId, subdomain);
            return contracts.Any();
        }
    }
}
