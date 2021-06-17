using Billing.Contracts.Domain.Clients.Interfaces;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Legacy
{
    public class LegacyClientsRemoteService : ILegacyClientsRemoteService
    {
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public LegacyClientsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Legacy clients api");
        }

        public Task SyncAsync()
        {
            var queryParams = new Dictionary<string, string>();
            return _httpClientHelper.GetObjectResponseAsync<object>("sync", queryParams);
        }
    }
}
