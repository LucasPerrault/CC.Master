using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public class SyncResult
    {
        public List<string> MissedTargets { get; }
        public List<Exception> Exceptions { get; }

        public SyncResult() : this(new List<string>(), new List<Exception>())
        { }

        public SyncResult(List<string> missedTargets, List<Exception> exceptions)
        {
            MissedTargets = missedTargets;
            Exceptions = exceptions;
        }
    }

    public interface IDataSourceSynchronizer
    {
        Task<SyncResult> SyncAsync(HashSet<string> targetsToIgnore);
        Task PurgeAsync();
    }
}
