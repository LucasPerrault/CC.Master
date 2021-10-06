using AdvancedFilters.Domain.DataSources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class LocalDataSourceSynchronizer<T> : IDataSourceSynchronizer
    {
        private readonly Func<Task<List<T>>> _getItemsAsyncAction;
        private readonly Func<List<T>, Task> _upsertAction;

        public LocalDataSourceSynchronizer(Func<Task<List<T>>> getItemsAsyncAction, Func<List<T>, Task> upsertAction)
        {
            _getItemsAsyncAction = getItemsAsyncAction;
            _upsertAction = upsertAction;
        }

        public async Task SyncAsync()
        {
            var items = await _getItemsAsyncAction();
            await _upsertAction(items);
        }
    }
}
