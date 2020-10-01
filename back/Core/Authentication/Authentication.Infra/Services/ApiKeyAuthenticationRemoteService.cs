using Authentication.Domain;
using Authentication.Infra.DTOs;
using Newtonsoft.Json;
using Partenaires.Infra.Configuration;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{
    public class ApiKeyAuthenticationRemoteService : RestApiV3HostRemoteService<ApiKeyPartenairesServiceConfiguration>
    {
        protected override string RemoteAppName => "Partenaires";

        public ApiKeyAuthenticationRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        public async Task<ApiKey> GetApiKeyPrincipalAsync(Guid token)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fields", LuccaApiKey.ApiFields }
            };

            try
            {
                var luccaUser = await GetObjectResponseAsync<LuccaApiKey>(token.ToString(), queryParams);

                var apiKey = luccaUser.Data;
                return new ApiKey { Token = apiKey.Token, Name = apiKey.Name };
            }
            catch
            {
                return null;
            }
        }
    }
}
