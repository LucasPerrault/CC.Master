using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.WsAuth
{
    public class WsAuthRemoteService
    {
        private readonly HttpClientHelper _httpClientHelper;

        public WsAuthRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new HttpClientHelper(httpClient, "Auth WS");
        }

        public Task PostAsync(string subroute)
        {
            return _httpClientHelper.PostAsync(subroute, new Dictionary<string, string>());
        }
    }
}
