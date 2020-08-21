using Core.Authentication.Infra.Configurations;
using System;

namespace Core.Authentication.Infra.Services
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
    }
}
