using Billing.Contracts.Domain.Clients.Interfaces;
using Core.Proxy.Infra.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Legacy
{
    public class LegacyClientsRemoteService : LegacyCcAuthenticatedRemoteService, ILegacyClientsRemoteService
    {
        public LegacyClientsRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal principal)
            : base(httpClient, jsonSerializer, principal)
        { }

        public Task SyncAsync()
        {
            var queryParams = new Dictionary<string, string>();
            return GetObjectResponseAsync<object>("sync", queryParams);
        }
    }
}
