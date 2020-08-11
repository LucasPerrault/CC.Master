using CloudControl.Shared.Domain.Authentication;
using CloudControl.Shared.Infra.Authentication.Configurations;
using CloudControl.Shared.Infra.Authentication.DTOs;
using CloudControl.Shared.Infra.Configuration;
using CloudControl.Shared.Infra.Remote.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudControl.Shared.Infra.Authentication
{
    public class AuthenticationRemoteService : HostRemoteService<PartenairesAuthServiceConfiguration>
    {
        private readonly AuthenticationConfiguration _authConfig;
        private readonly ApiKeysConfiguration _apiKeysConfig;
        private const string _authScheme = "Lucca";
        private const string _authType = "user";
        protected override string RemoteAppName => "Partenaires";

        public AuthenticationRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, AuthenticationConfiguration authConfig, ApiKeysConfiguration apiKeysConfig)
            : base(httpClient, jsonSerializer)
        {
            _authConfig = authConfig;
            _apiKeysConfig = apiKeysConfig;
        }

        public async Task<Principal> GetUserPrincipalAsync(Guid token)
        {
            var partenairesAuthConfig = new PartenairesAuthServiceConfiguration();
            partenairesAuthConfig.Authenticate(_httpClient, _authScheme, _authType, token);

            var queryParams = new Dictionary<string, string>() 
            { 
                { "fields", LuccaUser.ApiFields } 
            };

            try
            {
                var luccaUser = await GetObjectResponseAsync<LuccaUser>(queryParams);

                var user = luccaUser.Data.ToUser();
                return new Principal
                {
                    Token = token,
                    UserId = user.Id,
                    User = user
                };
            }
            catch
            {
                return null;
            }

        }

        public ApiKey GetApiKeyPrincipal(Guid token)
        {
            var config = _apiKeysConfig.SingleOrDefault(c => c.Token == token);
            if (config == null)
            {
                return null;
            }

            return ToApiKey(config);
        }

        private ApiKey ToApiKey(ApiKeyConfiguration config)
        {
            return new ApiKey
            {
                Id = config.Id,
                Name = config.Name,
                Token = config.Token
            };
        }
    }
}
