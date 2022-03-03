using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Sync;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Application
{
    public class Synchronizer
    {
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly IDataSyncService _dataSyncService;
        private readonly IFacetsSyncService _facetsSyncService;

        public Synchronizer
        (
            IEnvironmentsStore environmentsStore,
            IDataSyncService dataSyncService,
            IFacetsSyncService facetsSyncService
        )
        {
            _environmentsStore = environmentsStore;
            _dataSyncService = dataSyncService;
            _facetsSyncService = facetsSyncService;
        }

        public async Task SyncEverythingAsync()
        {
            //await SyncMultiTenantAsync();
            //await SyncMonoTenantAsync(new HashSet<string>());
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = new HashSet<string>{ "actronika" } });
            await _facetsSyncService.SyncTenantsFacetsAsync(environments, SyncStrategy.SyncEverything);
        }

        public async Task SyncMonoTenantAsync(HashSet<string> subdomains)
        {
            var syncStrategy = subdomains.Any()
                ? SyncStrategy.SyncSpecificEnvironmentsOnly
                : SyncStrategy.SyncEverything;
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });

            await _dataSyncService.SyncTenantsDataAsync(environments, syncStrategy);
            await _facetsSyncService.SyncTenantsFacetsAsync(environments, syncStrategy);
        }

        public async Task SyncRandomMonoTenantAsync(int tenantCount)
        {
            var environments = (await _environmentsStore.GetAsync(new EnvironmentFilter()))
                .Shuffle()
                .Take(tenantCount)
                .ToList();

            await _dataSyncService.SyncTenantsDataAsync(environments, SyncStrategy.SyncSpecificEnvironmentsOnly);
            await _facetsSyncService.SyncTenantsFacetsAsync(environments, SyncStrategy.SyncSpecificEnvironmentsOnly);
        }

        public Task SyncMultiTenantAsync()
        {
            return _dataSyncService.SyncMultiTenantDataAsync();
        }

        public async Task PurgeEverythingAsync()
        {
            await _facetsSyncService.PurgeEverythingAsync();
            await _dataSyncService.PurgeEverythingAsync();
        }

        public async Task PurgeTenantsAsync(HashSet<string> subdomains)
        {
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });

            await _facetsSyncService.PurgeTenantsAsync(environments);
            await _dataSyncService.PurgeTenantsDataAsync(environments);
        }
    }
}
