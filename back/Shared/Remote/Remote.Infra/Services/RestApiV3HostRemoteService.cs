using Newtonsoft.Json;
using Remote.Infra.Configurations;
using Remote.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remote.Infra.Services
{
    public abstract class RestApiV3HostRemoteService : HostRemoteService
    {
        protected RestApiV3HostRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        protected Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3Response<T>>(queryParams);
        }

        protected Task<RestApiV3Response<T>> GetObjectResponseAsync<T>(string id, Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3Response<T>>(id, queryParams);
        }

        protected Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3CollectionResponse<T>>(queryParams);
        }

        protected Task<RestApiV3CollectionResponse<T>> GetObjectCollectionResponseAsync<T>(string subRoute, Dictionary<string, string> queryParams)
        {
            return GetGenericObjectResponseAsync<RestApiV3CollectionResponse<T>>(subRoute, queryParams);
        }

        protected Task<RestApiV3Response<TResult>> PutObjectResponseAsync<TForm, TResult>(string id, TForm content, Dictionary<string, string> queryParams)
        {
            return PutGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(id, content, queryParams);
        }

        protected override string GetErrorMessage(JsonTextReader jsonTextReader)
        {
            var error = _jsonSerializer.Deserialize<RestApiV3Error>(jsonTextReader);
            return error.Message;
        }
    }
}
