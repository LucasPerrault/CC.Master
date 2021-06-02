using Instances.Application.Instances;
using Instances.Domain.Instances;
using Microsoft.Extensions.Logging;
using Remote.Infra.Extensions;
using Remote.Infra.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{

    public class UsersPasswordResetService : IUsersPasswordResetService
    {
        private const string _identityPasswordResetRoute = "/identity/api/users/resetallpasswords";

        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpClientOAuthAuthenticator _oAuthAuthenticator;
        private readonly IdentityAuthenticationConfig _identityAuthenticationConfig;
        private readonly ILogger<UsersPasswordResetService> _logger;

        public UsersPasswordResetService
        (
            IUsersPasswordHelper passwordHelper,
            IHttpClientFactory clientFactory,
            IHttpClientOAuthAuthenticator oAuthAuthenticator,
            IdentityAuthenticationConfig identityAuthenticationConfig,
            ILogger<UsersPasswordResetService> logger
        )
        {
            _passwordHelper = passwordHelper;
            _clientFactory = clientFactory;
            _oAuthAuthenticator = oAuthAuthenticator;
            _identityAuthenticationConfig = identityAuthenticationConfig;
            _logger = logger;
        }

        public async Task ResetPasswordAsync(Uri instanceHref, string password)
        {
            try
            {
                _passwordHelper.ThrowIfInvalid(password);
                var client = await GetAuthenticatedClientAsync(instanceHref);
                var uri = new Uri(instanceHref, _identityPasswordResetRoute);

                var dto = new PasswordResetPayload(password);
                using var payload = dto.ToJsonPayload();
                await client.PostAsync(uri, payload);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not reset password for instance {instanceHref}");
                throw;
            }
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync(Uri instanceHref)
        {
            var client = _clientFactory.CreateClient();
            client.WithUserAgent(nameof(UsersPasswordResetService));

            try
            {
                await _oAuthAuthenticator.AuthenticateAsync(client, new OAuthAuthentication
                {
                   Uri = new Uri(instanceHref, _identityAuthenticationConfig.TokenRequestRoute),
                   ClientId = _identityAuthenticationConfig.ClientId,
                   ClientSecret = _identityAuthenticationConfig.ClientSecret,
                   Scope = IdentityAuthenticationConfig.ImpersonationScope
                });

                return client;
            }
            catch (OAuthAuthenticationFailureException e)
            {
                throw new ApplicationException($"Could not authenticate {nameof(UsersPasswordResetService)} to identity", e);
            }
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
