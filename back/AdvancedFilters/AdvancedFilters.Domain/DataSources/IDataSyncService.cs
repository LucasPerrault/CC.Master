using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Domain.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSyncService
    {
        Task SyncMultiTenantDataAsync();
        Task SyncTenantsDataAsync(List<Environment> environments, SyncStrategy strategy);
        Task PurgeEverythingAsync();
        Task PurgeTenantsDataAsync(List<Environment> environments);
    }
}
