using Authentication.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Remote.Infra.Configurations
{
    public static class HttpClientExtensions
    {
        private const string _userAgentKey = "User-Agent";
        private const string _cloudControlUserAgent = "CloudControl";

        private static string GetFullUserAgent(string suffix) => $"{_cloudControlUserAgent} {suffix}";

        public static HttpClient WithUserAgent(this HttpClient httpClient, string userAgent)
        {
            httpClient.DefaultRequestHeaders.Add(_userAgentKey, GetFullUserAgent(userAgent));
            return httpClient;
        }

        public static HttpClient WithBaseAddress(this HttpClient httpClient, Uri host, string endpoint)
        {
            httpClient.BaseAddress = new Uri(host, endpoint);
            return httpClient;
        }

        public static HttpClient WithBaseAddress(this HttpClient httpClient, Uri host)
        {
            httpClient.BaseAddress = host;
            return httpClient;
        }

        public static HttpClientAuthenticator WithAuthScheme(this HttpClient httpClient, string scheme)
        {
            return new HttpClientAuthenticator(httpClient, scheme);
        }

        public class HttpClientAuthenticator
        {
            private readonly HttpClient _httpClient;
            private readonly string _scheme;

            internal HttpClientAuthenticator(HttpClient httpClient, string scheme)
            {
                _httpClient = httpClient;
                _scheme = scheme;
            }

            public HttpClient Authenticate(string parameter)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, parameter);
                return _httpClient;
            }

            public HttpClient AuthenticateAsUser(Guid appToken)
            {
                return Authenticate($"user={appToken}");
            }

            public HttpClient AuthenticateAsApplication(Guid appToken)
            {
                return Authenticate($"application={appToken}");
            }

            public HttpClient AuthenticateCurrentPrincipal(IServiceProvider provider)
            {
                var claimsPrincipal = provider.GetRequiredService<ClaimsPrincipal>();
                return claimsPrincipal switch
                {
                    CloudControlUserClaimsPrincipal u => AuthenticateAsUser(u.Token),
                    CloudControlApiKeyClaimsPrincipal ak => AuthenticateAsApplication(ak.Token),
                    _ => throw new ApplicationException("Can't authenticate to Lucca service with unrecognized principal")
                };
            }
        }
    }
}
