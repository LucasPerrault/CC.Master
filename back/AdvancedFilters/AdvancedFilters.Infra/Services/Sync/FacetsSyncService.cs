using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Facets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class FacetsSyncService : IFacetsSyncService
    {
        public FacetsSyncService()
        {
        }

        public async Task SyncTenantsFacetsAsync(List<Environment> environments, DataSyncStrategy syncSpecificEnvironmentsOnly)
        {
            
        }

        public async Task PurgeEverythingAsync()
        {
            
        }

        public async Task PurgeTenantsAsync(List<Environment> environments)
        {
            
        }
    }
}
