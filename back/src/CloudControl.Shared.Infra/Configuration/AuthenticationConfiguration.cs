using System;

namespace CloudControl.Shared.Infra.Configuration
{
    public class AuthenticationConfiguration
    {
        public Uri ServerUri { get; set; }

        public string EndpointPath { get; set; }
    }
}
