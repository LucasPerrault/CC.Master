using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSyncService
    {
        Task SyncMultiTenantDataAsync();
        Task SyncTenantsDataAsync(List<Environment> environments, DataSyncStrategy strategy);
        Task PurgeEverythingAsync();
        Task PurgeTenantsDataAsync(List<Environment> environments);
    }
}
