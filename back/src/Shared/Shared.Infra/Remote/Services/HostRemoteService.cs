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

        protected async Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            var response = await GetResponseMessageAsync("", queryParams);
            return await ParseResponseAsync<RestApiV3Response<T>, T>(response);
        }

        protected async Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(string id, Dictionary<string, string> queryParams)
        {
            var response = await GetResponseMessageAsync(id, queryParams);
            return await ParseResponseAsync<RestApiV3Response<T>, T>(response);
        }

        protected Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            return GetObjectCollectionResponseAsync<T>(string.Empty, queryParams);
        }

        protected async Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(string subRoute, Dictionary<string, string> queryParams)
        {
            var response = await GetResponseMessageAsync(subRoute, queryParams);
            return await ParseResponseAsync<RestApiV3CollectionResponse<T>, RestApiV3Collection<T>>(response);
        }

        protected async Task<RestApiV3Response<TResult>> PutObjectResponseAsync<Tform, TResult>(
            string id,
            Tform content,
            Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(id, queryParams);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);

            var jObject = JObject.FromObject(content);
            using var httpContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
            requestMessage.Content = httpContent;

            var response = await _httpClient.SendAsync(requestMessage);
            return await ParseResponseAsync<RestApiV3Response<TResult>, TResult>(response);
        }

        private Task<HttpResponseMessage> GetResponseMessageAsync(string subRoute, Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(subRoute, queryParams);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return _httpClient.SendAsync(requestMessage);
        }

        private async Task<TResponse> ParseResponseAsync<TResponse, U>(HttpResponseMessage response)
            where TResponse : RestApiV3Response<U>
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            if (!response.IsSuccessStatusCode)
            {
                var error = _jsonSerializer.Deserialize<RestApiV3Error>(jsonTextReader);
                throw new RemoteServiceException(RemoteAppName, response.StatusCode, error.Message);
            }

            return _jsonSerializer.Deserialize<TResponse>(jsonTextReader);
        }
    }
}
