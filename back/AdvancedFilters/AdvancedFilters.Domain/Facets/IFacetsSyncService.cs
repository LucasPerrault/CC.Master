using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Facets
{
    public interface IFacetsSyncService
    {
        Task SyncTenantsFacetsAsync(List<Environment> environments, DataSyncStrategy dataSyncStrategy);
        Task PurgeEverythingAsync();
        Task PurgeTenantsAsync(List<Environment> environments);
    }
}
