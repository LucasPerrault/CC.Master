using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class FetchJob<T>
    {
        public FetchJobHttpRequestDescription RequestDescription { get; }
        public Action<T> ObjectCreationFinalization { get; }

        public FetchJob(Uri uri, Action<HttpRequestMessage> authentication, Action<T> objectCreationFinalization)
        {
            RequestDescription = new FetchJobHttpRequestDescription(uri, authentication);
            ObjectCreationFinalization = objectCreationFinalization;
        }

        public class FetchJobHttpRequestDescription
        {
            private Uri Uri { get; }
            private Action<HttpRequestMessage> Authentication { get; }

            public FetchJobHttpRequestDescription(Uri uri, Action<HttpRequestMessage> authentication)
            {
                Uri = uri;
                Authentication = authentication;
            }

            public HttpRequestMessage ToRequest()
            {
                var requestMsg = new HttpRequestMessage(HttpMethod.Get, Uri);
                Authentication(requestMsg);
                return requestMsg;
            }
        }
    }

    public class DataSourceSynchronizer<TDto, T> : IDataSourceSynchronizer
        where TDto : IDto<T>
        where T : class
    {
        private readonly List<FetchJob<T>> _jobs;
        private readonly Func<List<T>, Task> _upsertAction;
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _fetchAction;

        public DataSourceSynchronizer
        (
            List<FetchJob<T>> jobs,
            Func<HttpRequestMessage, Task<HttpResponseMessage>> fetchAction,
            Func<List<T>, Task> upsertAction
        )
        {
            _jobs = jobs;
            _upsertAction = upsertAction;
            _fetchAction = fetchAction;
        }

        public async Task SyncAsync()
        {
            var items = await FetchAsync();
            await _upsertAction(items);
        }

        private async Task<List<T>> FetchAsync()
        {
            var items = new List<T>();
            foreach (var job in _jobs)
            {
                var batch = await FetchOneBatchAsync(job);
                foreach (var item in batch)
                {
                    job.ObjectCreationFinalization(item);
                }
                items.AddRange(batch);
            }

            return items;
        }

        private async Task<List<T>> FetchOneBatchAsync(FetchJob<T> job)
        {
            try
            {
                using var message = job.RequestDescription.ToRequest();
                using var response = await _fetchAction(message);
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync();

                var dto = await Serializer.DeserializeAsync<TDto>(stream);
                var batch = dto.ToItems();
                return batch;
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }
    }
}
