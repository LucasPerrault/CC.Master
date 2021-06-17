using Remote.Infra.Services;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using Salesforce.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace Salesforce.Infra.Services
{
    public class SalesforceAccountsRemoteService : ISalesforceAccountsRemoteService
    {
        private readonly HttpClientHelper _httpClientHelper;

        public SalesforceAccountsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new HttpClientHelper(httpClient, "Salesforce Service");
        }

        public Task UpdateAccountAsync(string clientSalesforceId, SalesforceAccount account)
        {
            var queryParams = new Dictionary<string, string>();
            return _httpClientHelper.PutGenericObjectResponseAsync<SalesforceAccount, SalesforceAccount>(clientSalesforceId, account, queryParams);
        }
    }
}
