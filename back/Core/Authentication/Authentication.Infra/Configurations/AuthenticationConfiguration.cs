using System;

namespace Authentication.Infra.Configurations
{
    public class AuthenticationConfiguration
    {
        public Uri ServerUri { get; set; }
        public string UsersEndpointPath { get; set; }
        public string ApiKeysEndpointPath { get; set; }
        public string RedirectEndpointPath { get; set; }
        public string LogoutEndpointPath { get; set; }
        public Guid ApiKeysFetcherToken { get; set; }
        public HangfireAuthenticationConfiguration Hangfire { get; set; }
    }

    public class HangfireAuthenticationConfiguration
    {
        public Guid SharedSecret { get; set; }
    }
}
