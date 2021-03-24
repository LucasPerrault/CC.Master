using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Remote.Infra.Exceptions;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remote.Infra.Services
{
    public abstract class HostRemoteService
    {
        private readonly HttpClient _httpClient;
        protected readonly JsonSerializer _jsonSerializer;

        protected abstract string RemoteApiDescription { get; }

        protected HostRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        protected Task<TResult> GetGenericObjectResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<TResult>(string.Empty, queryParams);
        }

        protected Task<TResult> GetGenericObjectResponseAsync<TResult>(string id, Dictionary<string, string> queryParams)
        {
            return GetRequestResponseAsync<TResult>(HttpMethod.Get, id, queryParams);
        }

        protected Task<TResult> PostGenericObjectResponseAsync<TForm, TResult>(TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, TResult>(string.Empty, content, queryParams);
        }

        protected Task<TResult> PostGenericObjectResponseAsync<TForm, TResult>(string urlSegment, TForm content, Dictionary<string, string> queryParams)
        {
            return GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod.Post, urlSegment, queryParams, content);
        }

        protected Task<TResult> PutGenericObjectResponseAsync<TForm, TResult>(string id, TForm content, Dictionary<string, string> queryParams)
        {
            return GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod.Put, id, queryParams, content);
        }

        protected abstract string GetErrorMessage(JsonTextReader jsonTextReader);

        protected void ApplyLateHttpClientAuthentication
        (
            string scheme,
            Action<HttpClientExtensions.HttpClientAuthenticator> authAction
        )
        {
            authAction(_httpClient.WithAuthScheme(scheme));
        }

        private async Task<TResult> GetRequestResponseAsync<TResult>(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var requestMessage = BuildHttpRequestMessage(method, subRoute, queryParams);

            var response = await _httpClient.SendAsync(requestMessage);
            return await ParseResponseAsync<TResult>(response);
        }

        private async Task<TResult> GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod method, string subRoute, Dictionary<string, string> queryParams, TForm content)
        {
            var requestMessage = BuildHttpRequestMessage(method, subRoute, queryParams);

            using var httpContent = content.ToJsonPayload();
            requestMessage.Content = httpContent;

            var response = await _httpClient.SendAsync(requestMessage);
            return await ParseResponseAsync<TResult>(response);
        }

        private HttpRequestMessage BuildHttpRequestMessage(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(subRoute, queryParams);
            return new HttpRequestMessage(method, requestUri);
        }

        private async Task<TResponse> ParseResponseAsync<TResponse>(HttpResponseMessage response)
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            if (!response.IsSuccessStatusCode)
            {
                var msg = GetErrorMessage(jsonTextReader);
                throw new RemoteServiceException(RemoteApiDescription, response.StatusCode, msg);
            }

            return _jsonSerializer.Deserialize<TResponse>(jsonTextReader);
        }
    }
}
