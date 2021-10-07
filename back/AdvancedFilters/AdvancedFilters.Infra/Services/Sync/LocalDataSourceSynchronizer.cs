using AdvancedFilters.Domain.DataSources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class LocalDataSourceSynchronizer<T> : IDataSourceSynchronizer
    {
        private readonly Func<Task<IReadOnlyCollection<T>>> _getItemsAsyncAction;
        private readonly Func<IReadOnlyCollection<T>, Task> _upsertAction;

        public LocalDataSourceSynchronizer(Func<Task<IReadOnlyCollection<T>>> getItemsAsyncAction, Func<IReadOnlyCollection<T>, Task> upsertAction)
        {
            _getItemsAsyncAction = getItemsAsyncAction;
            _upsertAction = upsertAction;
        }

        public async Task<SyncResult> SyncAsync(HashSet<string> targetsToIgnore)
        {
            // no target to ignore in local context
            var items = await _getItemsAsyncAction();
            await _upsertAction(items);
            return new SyncResult { MissedTargets = new List<string>() };
        }
    }
}
