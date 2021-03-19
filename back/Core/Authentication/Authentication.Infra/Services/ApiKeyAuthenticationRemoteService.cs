using Authentication.Domain;
using Authentication.Infra.DTOs;
using Newtonsoft.Json;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{
    public class ApiKeyAuthenticationRemoteService : RestApiV3HostRemoteService
    {
        private readonly AuthenticationCache _cache;
        protected override string RemoteApiDescription => "Partenaires";

        public ApiKeyAuthenticationRemoteService
        (
            HttpClient httpClient,
            JsonSerializer jsonSerializer,
            AuthenticationCache cache
        ) : base(httpClient, jsonSerializer)
        {
            _cache = cache;
        }

        public async Task<ApiKey> GetApiKeyPrincipalAsync(Guid token)
        {
            if (_cache.TryGetApiKey(token, out var cachedApiKey))
            {
                return cachedApiKey;
            }

            var queryParams = new Dictionary<string, string>
            {
                { "fields", LuccaApiKey.ApiFields }
            };

            var luccaUser = await GetObjectResponseAsync<LuccaApiKey>(token.ToString(), queryParams);
            var luccaApiKey = luccaUser.Data;

            if (luccaApiKey.Token == default || string.IsNullOrEmpty(luccaApiKey.Name))
            {
                return null;
            }

            var apiKey= new ApiKey { Token = luccaApiKey.Token, Name = luccaApiKey.Name };
            _cache.Cache(token, apiKey);
            return apiKey;
        }
    }
}
