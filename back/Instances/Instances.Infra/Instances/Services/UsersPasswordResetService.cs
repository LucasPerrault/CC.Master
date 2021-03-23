using IdentityModel.Client;
using Newtonsoft.Json;
using Remote.Infra.Extensions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{
    public interface IUsersPasswordResetService
    {
        Task ResetPasswordAsync(Uri uri, string password);
    }

    public class UsersPasswordResetService : IUsersPasswordResetService
    {
        private const string _mediaType = "application/json";
        private const string _identityPasswordResetRoute = "/identity/api/users/resetallpasswords";

        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IdentityAuthenticationConfig _identityAuthenticationConfig;

        public UsersPasswordResetService
        (
            IUsersPasswordHelper passwordHelper,
            IHttpClientFactory clientFactory,
            IdentityAuthenticationConfig identityAuthenticationConfig
        )
        {
            _passwordHelper = passwordHelper;
            _clientFactory = clientFactory;
            _identityAuthenticationConfig = identityAuthenticationConfig;
        }

        public async Task ResetPasswordAsync(Uri instanceHref, string password)
        {
            _passwordHelper.ThrowIfInvalid(password);
            var client = await GetAuthenticatedClientAsync(instanceHref);
            var uri = new Uri(instanceHref, _identityPasswordResetRoute);
            var payload = GetPayload(password);
            await client.PostAsync(uri, payload);
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync(Uri instanceHref)
        {
            var client = _clientFactory.CreateClient();
            client.WithUserAgent(nameof(UsersPasswordResetService));

            var uri = new Uri(instanceHref, _identityAuthenticationConfig.TokenRequestRoute);
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = uri.ToString(),
                ClientId = _identityAuthenticationConfig.ClientId,
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                ClientSecret = _identityAuthenticationConfig.ClientSecret,
                Scope = IdentityAuthenticationConfig.ImpersonationScope
            });

            if (tokenResponse.IsError)
            {
                throw new ApplicationException($"Could not authenticate {nameof(UsersPasswordResetService)} to identity");
            }

            client.SetBearerToken(tokenResponse.AccessToken);
            return client;
        }

        private StringContent GetPayload(string password)
        {
            var payload = new PasswordResetPayload(password);
            var payloadToString = JsonConvert.SerializeObject(payload);
            return new StringContent(payloadToString, Encoding.UTF8, _mediaType);
        }

        private class PasswordResetPayload
        {
            public string Password { get; }

            public PasswordResetPayload(string password)
            {
                Password = password;
            }
        }
    }
}
