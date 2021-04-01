using System;

namespace Instances.Infra.WsAuth
{
    public class WsAuthConfiguration
    {
        public Uri ServerUri { get; set; }
        public string EndpointPath { get; set; }
        public Guid Token { get; set; }
    }
}
