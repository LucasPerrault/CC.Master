using System;

namespace Instances.Infra.WsAuth
{
    public class WsAuthConfiguration
    {
        // lucca config will not provide a host, but a host with api version :
        // login.ilucca.net/sync-v2
        public string ServerApiEndpoint { get; set; }
        public Guid Token { get; set; }
    }
}
