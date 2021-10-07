using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public class SyncResult
    {
        public List<string> MissedTargets { get; set; }
    }

    public interface IDataSourceSynchronizer
    {
        Task<SyncResult> SyncAsync(HashSet<string> targetsToIgnore);
    }
}
