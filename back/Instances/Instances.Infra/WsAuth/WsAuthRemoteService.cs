using Newtonsoft.Json;
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

        public WsAuthRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        public Task PostAsync(string subroute)
        {
            return PostAsync(subroute, new Dictionary<string, string>());
        }

        protected override string GetErrorMessage(JsonTextReader jsonTextReader)
        {
            throw new NotImplementedException();
        }
    }
}
