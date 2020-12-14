using Newtonsoft.Json;
using Remote.Infra.Services;
using Salesforce.Domain.Interfaces;
using Salesforce.Domain.Models;
using Salesforce.Infra.Configurations;
using Salesforce.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Salesforce.Infra.Services
{
    public class SalesforceAccountsRemoteService : HostRemoteService<SalesforceServiceConfiguration>, ISalesforceAccountsRemoteService
    {
        protected override string RemoteApiDescription => "Salesforce Service";

        public SalesforceAccountsRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        public Task UpdateAccountAsync(string clientSalesforceId, SalesforceAccount account)
        {
            var queryParams = new Dictionary<string, string>();
            return PutGenericObjectResponseAsync<SalesforceAccount, SalesforceAccount>(clientSalesforceId, account, queryParams);
        }

        protected override string GetErrorMessage(JsonTextReader jsonTextReader)
        {
            var error = _jsonSerializer.Deserialize<SalesforceErrorDto>(jsonTextReader);
            return error?.Message;
        }
    }
}
