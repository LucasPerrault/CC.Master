using Billing.Contracts.Domain.Clients.Interfaces;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Legacy
{
    public class LegacyClientsRemoteService : RestApiV3HostRemoteService, ILegacyClientsRemoteService
    {
        protected override string RemoteApiDescription => "Legacy clients api";
        public LegacyClientsRemoteService(HttpClient httpClient)
            : base(httpClient)
        { }

        public Task SyncAsync()
        {
            var queryParams = new Dictionary<string, string>();
            return GetObjectResponseAsync<object>("sync", queryParams);
        }
    }
}
