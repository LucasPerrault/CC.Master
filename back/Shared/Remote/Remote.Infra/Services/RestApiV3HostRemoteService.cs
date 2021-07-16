﻿using Remote.Infra.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace Remote.Infra.Services
{
    public abstract class RestApiV3HostRemoteService : HostRemoteService
    {
        protected RestApiV3HostRemoteService(HttpClient httpClient)
            : base(httpClient)
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

        protected Task<RestApiV3Response<TResult>> PostObjectResponseAsync<TForm, TResult>(string urlSegment, TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(urlSegment, content, queryParams);
        }

        protected Task<RestApiV3Response<TResult>> PostObjectResponseAsync<TForm, TResult>(TForm content, Dictionary<string, string> queryParams)
        {
            return PostGenericObjectResponseAsync<TForm, RestApiV3Response<TResult>>(content, queryParams);
        }

        protected override string GetErrorMessage(string s)
        {
            var error = Serializer.Deserialize<RestApiV3Error>(s);
            return error.Message;
        }
    }
}
