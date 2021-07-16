using Microsoft.AspNetCore.WebUtilities;
using Remote.Infra.Exceptions;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;
using HttpClientAuthenticator = Remote.Infra.Extensions.HttpClientExtensions.HttpClientAuthenticator;

namespace Remote.Infra.Services
{
    public class HttpClientHelper<TError> where TError : class
    {
        private readonly HttpClient _httpClient;
        private readonly Func<TError, string> _errorMessageFunc;

        private string _remoteApiDescription { get; }

        public HttpClientHelper(HttpClient httpClient, string remoteApiDescription, Func<TError, string> errorMessageFunc)
        {
            _httpClient = httpClient;
            _remoteApiDescription = remoteApiDescription;
            _errorMessageFunc = errorMessageFunc;
        }

        public Task<TResult> GetGenericObjectResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<TResult>(string.Empty, queryParams);
        }

        public Task<TResult> GetGenericObjectResponseAsync<TResult>(string id, Dictionary<string, string> queryParams)
        {
            return GetRequestResponseResultAsync<TResult>(HttpMethod.Get, id, queryParams);
        }

        public Task PostAsync(Dictionary<string, string> queryParams)
        {
            return PostAsync(string.Empty, queryParams);
        }

        public async Task PostAsync(string urlSegment, Dictionary<string, string> queryParams)
        {
            await GetRequestResponseMessageAsync(HttpMethod.Post, urlSegment, queryParams);
        }

        public Task<TResult> PostResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return PostResponseAsync<TResult>(string.Empty, queryParams);
        }

        public Task<TResult> PostResponseAsync<TResult>(string urlSegment, Dictionary<string, string> queryParams)
        {
            return GetRequestResponseResultAsync<TResult>(HttpMethod.Post, urlSegment, queryParams);
        }

        public Task<TResult> PostGenericObjectResponseAsync<TForm, TResult>(TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, TResult>(string.Empty, content, queryParams);
        }

        public Task<TResult> PostGenericObjectResponseAsync<TForm, TResult>(string urlSegment, TForm content, Dictionary<string, string> queryParams)
        {
            return GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod.Post, urlSegment, queryParams, content);
        }

        public Task<TResult> PutGenericObjectResponseAsync<TForm, TResult>(string id, TForm content, Dictionary<string, string> queryParams)
        {
            return GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod.Put, id, queryParams, content);
        }

        public Task<TResult> PutGenericObjectResponseAsync<TForm, TResult>(string id, TForm content)
        {
            return PutGenericObjectResponseAsync<TForm, TResult>(id, content, new Dictionary<string, string>());
        }

        public Task<HttpResponseMessage> GetRequestResponseMessageAsync(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var requestMessage = BuildHttpRequestMessage(method, subRoute, queryParams);

            return _httpClient.SendAsync(requestMessage);
        }

        public async Task<TResult> GetRequestResponseResultAsync<TResult>(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var response = await GetRequestResponseMessageAsync(method, subRoute, queryParams);

            return await ParseResponseAsync<TResult>(response);
        }

        public async Task<TResult> GetRequestWithContentResponseAsync<TForm, TResult>(HttpMethod method, string subRoute, Dictionary<string, string> queryParams, TForm content)
        {
            var requestMessage = BuildHttpRequestMessage(method, subRoute, queryParams);

            using var httpContent = content.ToJsonPayload();
            requestMessage.Content = httpContent;

            var response = await _httpClient.SendAsync(requestMessage);
            return await ParseResponseAsync<TResult>(response);
        }

        public HttpRequestMessage BuildHttpRequestMessage(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var requestUri = QueryHelpers.AddQueryString(subRoute, queryParams);
            return new HttpRequestMessage(method, requestUri);
        }

        public void ApplyLateHttpClientAuthentication(string scheme, Action<HttpClientAuthenticator> authAction)
        {
            authAction(_httpClient.WithAuthScheme(scheme));
        }

        private async Task<TResponse> ParseResponseAsync<TResponse>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Serializer.Deserialize<TResponse>(responseString);
            }

            var errorMessage = _errorMessageFunc(Serializer.Deserialize<TError>(responseString));
            throw new RemoteServiceException(_remoteApiDescription, response.StatusCode, errorMessage);

        }
    }
}
