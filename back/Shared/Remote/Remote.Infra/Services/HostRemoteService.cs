using Microsoft.AspNetCore.WebUtilities;
using Remote.Infra.Exceptions;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tools;
using HttpClientExtensions = Remote.Infra.Extensions.HttpClientExtensions;

namespace Remote.Infra.Services
{
    public abstract class HostRemoteService
    {
        private readonly HttpClient _httpClient;

        protected abstract string RemoteApiDescription { get; }

        protected HostRemoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected Task<TResult> GetGenericObjectResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<TResult>(string.Empty, queryParams);
        }

        protected Task<TResult> GetGenericObjectResponseAsync<TResult>(string id, Dictionary<string, string> queryParams)
        {
            return GetRequestResponseResultAsync<TResult>(HttpMethod.Get, id, queryParams);
        }

        protected Task PostAsync(Dictionary<string, string> queryParams)
        {
            return PostAsync(string.Empty, queryParams);
        }

        protected async Task PostAsync(string urlSegment, Dictionary<string, string> queryParams)
        {
            await GetRequestResponseMessageAsync(HttpMethod.Post, urlSegment, queryParams);
        }

        protected Task<TResult> PostResponseAsync<TResult>(Dictionary<string, string> queryParams)
        {
            return PostResponseAsync<TResult>(string.Empty, queryParams);
        }

        protected Task<TResult> PostResponseAsync<TResult>(string urlSegment, Dictionary<string, string> queryParams)
        {
            return GetRequestResponseResultAsync<TResult>(HttpMethod.Post, urlSegment, queryParams);
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

        protected abstract string GetErrorMessage(string responseAsString);

        protected void ApplyLateHttpClientAuthentication
        (
            string scheme,
            Action<HttpClientExtensions.HttpClientAuthenticator> authAction
        )
        {
            authAction(_httpClient.WithAuthScheme(scheme));
        }

        private Task<HttpResponseMessage> GetRequestResponseMessageAsync(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var requestMessage = BuildHttpRequestMessage(method, subRoute, queryParams);

            return _httpClient.SendAsync(requestMessage);
        }

        private async Task<TResult> GetRequestResponseResultAsync<TResult>(HttpMethod method, string subRoute, Dictionary<string, string> queryParams)
        {
            var response = await GetRequestResponseMessageAsync(method, subRoute, queryParams);

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
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new RemoteServiceException(RemoteApiDescription, response.StatusCode, GetErrorMessage(responseString));
            }

            return Serializer.Deserialize<TResponse>(responseString);
        }
    }
}
