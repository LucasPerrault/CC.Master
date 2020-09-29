using Authentication.Infra.Configurations;
using System;

namespace Authentication.Infra.Services
{
    public class AuthRedirectionRemoteService
    {
        private readonly AuthenticationConfiguration _authConfig;

        public AuthRedirectionRemoteService(AuthenticationConfiguration authConfig)
        {
            _authConfig = authConfig;
        }

        public Uri GetAuthRedirectionUri(string callbackPath)
        {
            return new Uri(_authConfig.ServerUri, $"{_authConfig.RedirectEndpointPath}?callback={callbackPath}");
        }

        public Uri GetLogoutRedirectionUri(string callbackPath)
        {
            return new Uri(_authConfig.ServerUri, $"{_authConfig.LogoutEndpointPath}?callback={callbackPath}");
        }
    }
}
