using Authentication.Domain;
using Authentication.Infra.DTOs;
using Cache.Abstractions;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{

    public class ApiKeyInMemoryCache : InMemoryCache<Guid, ApiKey>
    {
        public ApiKeyInMemoryCache() : base(TimeSpan.FromSeconds(10))
        { }
    }

    public interface IApiKeyAuthenticationRemoteService
    {
        Task<ApiKey> GetApiKeyPrincipalAsync(Guid token);
    }

    public class ApiKeyAuthenticationRemoteService : IApiKeyAuthenticationRemoteService
    {
        private readonly ApiKeyInMemoryCache _cache;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public ApiKeyAuthenticationRemoteService(HttpClient httpClient, ApiKeyInMemoryCache cache)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires");
            _cache = cache;
        }

        public async Task<ApiKey> GetApiKeyPrincipalAsync(Guid token)
        {
            if (_cache.TryGet(token, out var cachedApiKey))
            {
                return cachedApiKey;
            }

            var queryParams = new Dictionary<string, string>
            {
                { "fields", LuccaApiKey.ApiFields }
            };

            var luccaUser = await _httpClientHelper.GetObjectResponseAsync<LuccaApiKey>(token.ToString(), queryParams);
            var luccaApiKey = luccaUser.Data;

            if (luccaApiKey.Token == default || string.IsNullOrEmpty(luccaApiKey.Name))
            {
                return null;
            }

            var apiKey = new ApiKey { Token = luccaApiKey.Token, Name = luccaApiKey.Name };
            _cache.Cache(token, apiKey);
            return apiKey;
        }
    }
}
