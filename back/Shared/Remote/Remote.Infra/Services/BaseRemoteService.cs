using Remote.Infra.Configurations;
using System;
using System.Net.Http;

namespace Remote.Infra.Services
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
