using Authentication.Domain;
using Authentication.Infra.Configurations;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{
    public class AuthRedirectionRemoteService
    {
        private readonly AuthenticationConfiguration _authConfig;
        private readonly ClaimsPrincipal _principal;
        private readonly HttpClient _httpClient;

        public AuthRedirectionRemoteService(AuthenticationConfiguration authConfig, ClaimsPrincipal principal, HttpClient httpClient)
        {
            _authConfig = authConfig;
            _principal = principal;
            _httpClient = httpClient;
        }

        public Uri GetAuthRedirectionUri(string callbackPath)
        {
            return new Uri(_authConfig.ServerUri, $"{_authConfig.RedirectEndpointPath}?callback={callbackPath}");
        }

        public async Task LogoutAsync()
        {
            if (_principal is CloudControlUserClaimsPrincipal)
            {
                await _httpClient.PostAsync(GetLogoutUri(), new StringContent(""));
            }
        }

        private Uri GetLogoutUri()
        {
            return new Uri(_authConfig.ServerUri, $"{_authConfig.LogoutEndpointPath}");
        }
    }
}
