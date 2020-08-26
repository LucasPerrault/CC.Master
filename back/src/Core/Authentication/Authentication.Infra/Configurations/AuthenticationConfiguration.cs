using System;

namespace Authentication.Infra.Configurations
{
    public class AuthenticationConfiguration
    {
        public Uri ServerUri { get; set; }

        public string EndpointPath { get; set; }

        public string RedirectEndpointPath { get; set; }
    }
}
