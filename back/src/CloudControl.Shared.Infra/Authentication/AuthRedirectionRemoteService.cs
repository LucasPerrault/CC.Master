using CloudControl.Shared.Infra.Configuration;
using System;

namespace CloudControl.Shared.Infra.Authentication
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
