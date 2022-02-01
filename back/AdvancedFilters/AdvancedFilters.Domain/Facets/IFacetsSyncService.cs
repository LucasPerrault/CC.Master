using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Domain.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Facets
{
    public interface IFacetsSyncService
    {
        Task SyncTenantsFacetsAsync(List<Environment> environments, SyncStrategy syncStrategy);
        Task PurgeEverythingAsync();
        Task PurgeTenantsAsync(List<Environment> environments);
    }
}
