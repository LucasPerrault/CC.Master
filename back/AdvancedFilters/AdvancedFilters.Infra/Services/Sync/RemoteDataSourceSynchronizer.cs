using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class FetchJob<T>
    {
        public string TargetCode { get; }
        public FetchJobHttpRequestDescription RequestDescription { get; }
        public Action<T> ObjectCreationFinalization { get; }

        public FetchJob(Uri uri, Action<HttpRequestMessage> authentication, Action<T> objectCreationFinalization, string targetCode)
        {
            RequestDescription = new FetchJobHttpRequestDescription(uri, authentication);
            ObjectCreationFinalization = objectCreationFinalization;
            TargetCode = targetCode;
        }

        public class FetchJobHttpRequestDescription
        {
            public Uri Uri { get; }
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

    public class RemoteDataSourceSynchronizer<TDto, T> : IDataSourceSynchronizer
        where TDto : IDto<T>
        where T : class
    {
        private readonly List<FetchJob<T>> _jobs;
        private readonly Func<List<T>, Task> _upsertAction;
        private readonly ILogger _logger;
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _fetchAction;

        public RemoteDataSourceSynchronizer
        (
            List<FetchJob<T>> jobs,
            Func<HttpRequestMessage, Task<HttpResponseMessage>> fetchAction,
            Func<List<T>, Task> upsertAction,
            ILogger logger
        )
        {
            _jobs = jobs;
            _upsertAction = upsertAction;
            _logger = logger;
            _fetchAction = fetchAction;
        }

        public async Task<SyncResult> SyncAsync(HashSet<string> targetsToIgnore)
        {
            var fetchResult = await FetchAsync(targetsToIgnore);
            await _upsertAction(fetchResult.Items);
            return new SyncResult { MissedTargets = fetchResult.Failures.Select(f => f.TargetCode).ToList() };
        }

        private async Task<FetchResult> FetchAsync(HashSet<string> targetsToIgnore)
        {
            var result = new FetchResult { Items = new List<T>(), Failures = new List<FetchJob<T>>()};
            var batches = _jobs.Where(j => !targetsToIgnore.Contains(j.TargetCode)).Batch(20);
            foreach (var jobBatch in batches)
            {
                var batchResults = await Task.WhenAll(jobBatch.Select(FetchOneBatchAsync));
                foreach (var batchResult in batchResults)
                {
                    if (!batchResult.Success)
                    {
                        result.Failures.Add(batchResult.FetchJob);
                        continue;
                    }

                    foreach (var item in batchResult.Items)
                    {
                        batchResult.FetchJob.ObjectCreationFinalization(item);
                    }
                    result.Items.AddRange(batchResult.Items);
                }
            }

            return result;
        }

        private async Task<BatchFetchResult> FetchOneBatchAsync(FetchJob<T> job)
        {
            var result = new BatchFetchResult { FetchJob = job, Success = false };
            try
            {
                using var message = job.RequestDescription.ToRequest();
                using var response = await _fetchAction(message);
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync();

                var dto = await Serializer.DeserializeAsync<TDto>(stream);
                var batch = dto.ToItems();
                result.Items = batch;
                result.Success = true;
            }
            catch (Exception e)
            {
                var exception = new FetchJobException(job, e);
                _logger.LogError(exception, "Fetch failed");
            }

            return result;
        }

        public class FetchResult
        {
            public List<T> Items { get; set; }
            public List<FetchJob<T>> Failures { get; set; }
        }

        internal class BatchFetchResult
        {
            public List<T> Items { get; set; }
            public bool Success { get; set; }
            public FetchJob<T> FetchJob { get; set; }
        }

        public class FetchJobException : ApplicationException
        {
            public FetchJobException(FetchJob<T> fetchJob, Exception e) : base($"DataSource fetch failed for {typeof(T).Name} on {fetchJob.RequestDescription.Uri}", e)
            { }
        }
    }
}
