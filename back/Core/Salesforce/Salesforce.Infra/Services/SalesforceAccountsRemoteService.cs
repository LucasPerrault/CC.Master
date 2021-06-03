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
    public class SalesforceAccountsRemoteService : HostRemoteService, ISalesforceAccountsRemoteService
    {
        protected override string RemoteApiDescription => "Salesforce Service";

        public SalesforceAccountsRemoteService(HttpClient httpClient) : base(httpClient)
        { }

        public Task UpdateAccountAsync(string clientSalesforceId, SalesforceAccount account)
        {
            var queryParams = new Dictionary<string, string>();
            return PutGenericObjectResponseAsync<SalesforceAccount, SalesforceAccount>(clientSalesforceId, account, queryParams);
        }

        protected override string GetErrorMessage(string s)
        {
            var error = Serializer.Deserialize<SalesforceErrorDto>(s);
            return error?.Message;
        }
    }
}
