using Remote.Infra.Services;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using Salesforce.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Salesforce.Infra.Services
{

    public class SalesforceAccountsRemoteService : ISalesforceAccountsRemoteService
    {
        private readonly HttpClientHelper<SalesforceErrorDto> _httpClientHelper;

        public SalesforceAccountsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new HttpClientHelper<SalesforceErrorDto>(httpClient, "Salesforce Service", e => e.Message);
        }

        public Task UpdateAccountAsync(string clientSalesforceId, SalesforceAccount account)
        {
            return _httpClientHelper.PutGenericObjectResponseAsync<SalesforceAccount, SalesforceAccount>(clientSalesforceId, account);
        }
    }
}
