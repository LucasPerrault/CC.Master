using Shared.Infra.Remote.Configurations;
using System;
using System.Net.Http;

namespace Shared.Infra.Remote.Services
{
    public abstract class BaseRemoteService<T> where T : IHttpClientConfiguration
    {
        protected readonly HttpClient _httpClient;

        public BaseRemoteService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
    }
}
