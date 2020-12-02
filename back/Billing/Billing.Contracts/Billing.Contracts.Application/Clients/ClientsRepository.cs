using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Exceptions;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Clients
{
    public class ClientsRepository
    {
        private readonly IClientsStore _clientsStore;
        private readonly ILegacyClientsRemoteService _legacyClientsRemoteService;
        private readonly IClientVisibilityService _clientVisibilityService;
        private readonly ISalesforceAccountsRemoteService _salesforceAccountsRemoteService;

        public ClientsRepository(
            IClientsStore clientsStore,
            ILegacyClientsRemoteService legacyClientsRemoteService,
            IClientVisibilityService clientVisibilityService,
            ISalesforceAccountsRemoteService salesforceAccountsRemoteService)
        {
            _clientsStore = clientsStore ?? throw new ArgumentNullException(nameof(clientsStore));
            _legacyClientsRemoteService = legacyClientsRemoteService ?? throw new ArgumentNullException(nameof(legacyClientsRemoteService));
            _clientVisibilityService = clientVisibilityService ?? throw new ArgumentNullException(nameof(clientVisibilityService));
            _salesforceAccountsRemoteService = salesforceAccountsRemoteService ?? throw new ArgumentNullException(nameof(salesforceAccountsRemoteService));
        }

        public async Task PutAsync(Guid externalId, Client client, string subdomain)
        {
            var isClientVisible = await _clientVisibilityService.IsClientVisibleInSubdomainAsync(externalId, subdomain);
            if (!isClientVisible)
            {
                throw new ClientNotVisibleException();
            }

            var currentClient = await _clientsStore.GetByExternalIdAsync(externalId);
            var clientSfId = currentClient.SalesforceId;

            var account = ConvertToAccount(client);
            await _salesforceAccountsRemoteService.UpdateAccountAsync(clientSfId, account);

            await _legacyClientsRemoteService.SyncAsync();
        }

        private SalesforceAccount ConvertToAccount(Client client)
        {
            return new SalesforceAccount
            {
                Id = client.SalesforceId,
                BillingStreet = client.BillingStreet,
                BillingCity = client.BillingCity,
                BillingPostalCode = client.BillingPostalCode,
                BillingState = client.BillingState,
                BillingCountry = client.BillingCountry,
                Email_de_facturation__c = client.BillingMail,
                Phone = client.Phone
            };
        }
    }
}
