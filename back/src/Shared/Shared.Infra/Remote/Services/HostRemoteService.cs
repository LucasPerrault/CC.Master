using Shared.Infra.Remote.Configurations;
using Shared.Infra.Remote.DTOs;
using Shared.Infra.Remote.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Shared.Infra.Remote.Services
{
    public abstract class HostRemoteService<TC> : BaseRemoteService<HostHttpClientConfiguration>
        where TC : RemoteServiceConfiguration<HostHttpClientConfiguration>
    {
        protected readonly JsonSerializer _jsonSerializer;

        protected abstract string RemoteAppName { get; }

        protected HostRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        protected Task<TResult> GetGenericObjectResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<TResult>(string.Empty, queryParams);
        }

        protected async Task<TResult> GetGenericObjectResponseAsync<TResult>(string id, Dictionary<string, string> queryParams)
        {
            var response = await GetResponseMessageAsync(id, queryParams);
            return await ParseResponseAsync<TResult>(response);
        }

        protected async Task<TResult> PutGenericObjectResponseAsync<TForm, TResult>(string id, TForm content, Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(id, queryParams);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);

            var jObject = JObject.FromObject(content);
            using var httpContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
            requestMessage.Content = httpContent;

            var response = await _httpClient.SendAsync(requestMessage);
            return await ParseResponseAsync<TResult>(response);
        }

        protected abstract string GetErrorMessage(JsonTextReader jsonTextReader);

        private Task<HttpResponseMessage> GetResponseMessageAsync(string subRoute, Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(subRoute, queryParams);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return _httpClient.SendAsync(requestMessage);
        }

        private async Task<TResponse> ParseResponseAsync<TResponse>(HttpResponseMessage response)
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            if (!response.IsSuccessStatusCode)
            {
                var msg = GetErrorMessage(jsonTextReader);
                throw new RemoteServiceException(RemoteAppName, response.StatusCode, msg);
            }

            return _jsonSerializer.Deserialize<TResponse>(jsonTextReader);
        }
    }
}
