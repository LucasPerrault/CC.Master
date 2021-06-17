using Remote.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remote.Infra.Services
{
    public class RestApiV3HttpClientHelper : HttpClientHelper<RestApiV3Error>
    {
        public RestApiV3HttpClientHelper(HttpClient httpClient, string remoteApiDescription)
            : base(httpClient, remoteApiDescription, e => e.Message)
        { }

        public Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3Response<T>>(queryParams);
        }

        public Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(string id)
        {
            return GetObjectResponseAsync<T>(id, new Dictionary<string, string>());
        }

        public Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(string id, Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3Response<T>>(id, queryParams);
        }

        public Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3CollectionResponse<T>>(queryParams);
        }

        public Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>()
        {
            return GetObjectCollectionResponseAsync<T>(new Dictionary<string, string>());
        }

        public Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(string subRoute, Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3CollectionResponse<T>>(subRoute, queryParams);
        }

        public Task<RestApiV3Response<TResult>> PutObjectResponseAsync<TForm, TResult>(string id, TForm content, Dictionary<string, string> queryParams)
        {
            return PutGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(id, content, queryParams);
        }

        public Task<RestApiV3Response<TResult>> PostObjectResponseAsync<TForm, TResult>(string urlSegment, TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(urlSegment, content, queryParams);
        }

        public Task<RestApiV3Response<TResult>> PostObjectResponseAsync<TForm, TResult>(TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(content, queryParams);
        }
    }
}
