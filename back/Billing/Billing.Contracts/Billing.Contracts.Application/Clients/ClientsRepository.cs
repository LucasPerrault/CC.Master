using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Exceptions;
using Lucca.Core.Api.Abstractions.Fields;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Abstractions.Sorting;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Clients
{
    public class ClientsRepository
    {
        private readonly IClientsStore _clientsStore;
        private readonly ILegacyClientsRemoteService _legacyClientsRemoteService;
        private readonly ClientRightFilter _clientRightFilter;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly ISalesforceAccountsRemoteService _salesforceAccountsRemoteService;

        public ClientsRepository(
            IClientsStore clientsStore,
            ILegacyClientsRemoteService legacyClientsRemoteService,
            ClientRightFilter clientRightFilter,
            ClaimsPrincipal claimsPrincipal,
            ISalesforceAccountsRemoteService salesforceAccountsRemoteService)
        {
            _clientsStore = clientsStore;
            _legacyClientsRemoteService = legacyClientsRemoteService;
            _clientRightFilter = clientRightFilter;
            _claimsPrincipal = claimsPrincipal;
            _salesforceAccountsRemoteService = salesforceAccountsRemoteService;
        }

        public async Task<Page<Client>> GetPageAsync(IPageToken pageToken, ClientFilter clientFilter)
        {
            var accessRight = await _clientRightFilter.GetReadAccessAsync(_claimsPrincipal);
            return await _clientsStore.GetPageAsync(pageToken, accessRight, clientFilter);
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            var pageToken = new NumberPageToken(SortingQuery.FromRawValue(nameof(Client.Id)), 1, 2, RootFields.None);
            var page = await GetPageAsync(pageToken, new ClientFilter { Id = id });

            var clients = page.Items.ToList();
            if (!clients.Any())
            {
                throw new ClientNotVisibleException();
            }

            return clients.Single();
        }

        public async Task PutAsync(Guid externalId, Client client, string subdomain)
        {
            var accessRight = _clientRightFilter.GetAccessForEnvironment(subdomain);
            var filter = new ClientFilter { ExternalId = externalId };

            var clients = await _clientsStore.GetAsync(accessRight, filter);
            var currentClient = clients.SingleOrDefault();
            if (currentClient == null)
            {
                throw new ClientNotVisibleException();
            }

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
