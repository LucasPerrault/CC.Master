using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remote.Infra.Services
{
    public class OAuthAuthentication
    {
        public Uri Uri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
    public interface IHttpClientOAuthAuthenticator
    {
        Task AuthenticateAsync(HttpClient client, OAuthAuthentication authentication);
    }

    public class HttpClientOAuthAuthenticator : IHttpClientOAuthAuthenticator
    {
        public async Task AuthenticateAsync(HttpClient client, OAuthAuthentication authentication)
        {
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = authentication.Uri.ToString(),
                ClientId = authentication.ClientId,
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                ClientSecret = authentication.ClientSecret,
                Scope = authentication.Scope
            });

            if (tokenResponse.IsError)
            {
                throw new OAuthAuthenticationFailureException();
            }

            client.SetBearerToken(tokenResponse.AccessToken);
        }
    }

    public class OAuthAuthenticationFailureException : ApplicationException
    { }
}
