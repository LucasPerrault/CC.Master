using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.WsAuth
{
    public class WsAuthRemoteService : HostRemoteService
    {
        protected override string RemoteApiDescription => "Auth WS";

        public WsAuthRemoteService(HttpClient httpClient) : base(httpClient)
        { }

        public Task PostAsync(string subroute)
        {
            return PostAsync(subroute, new Dictionary<string, string>());
        }

        protected override string GetErrorMessage(string s)
        {
            throw new NotImplementedException();
        }
    }
}
