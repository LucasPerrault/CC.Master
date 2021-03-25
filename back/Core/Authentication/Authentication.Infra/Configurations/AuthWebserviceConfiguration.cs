using System;

namespace Authentication.Infra.Configurations
{
    public class AuthWebserviceConfiguration
    {
        public Uri ServerUri { get; set; }
        public string SyncEndpointPath { get; set; }
        public Guid Token { get; set; }
    }
}
