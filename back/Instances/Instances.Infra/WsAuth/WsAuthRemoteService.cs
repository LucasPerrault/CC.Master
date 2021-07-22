using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.WsAuth
{
    internal class WsAuthErrorMessage
    {
        public string Details { get; set; }
    }

    public class WsAuthRemoteService
    {
        private readonly HttpClientHelper<WsAuthErrorMessage> _httpClientHelper;

        public WsAuthRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new HttpClientHelper<WsAuthErrorMessage>(httpClient, "Auth WS", m => m.Details);
        }

        public Task PostAsync(string subroute)
        {
            return _httpClientHelper.PostAsync(subroute, new Dictionary<string, string>());
        }
    }
}
