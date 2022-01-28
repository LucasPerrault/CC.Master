using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
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

        public Synchronizer
        (
            IEnvironmentsStore environmentsStore,
            IDataSyncService dataSyncService,
        )
        {
            _environmentsStore = environmentsStore;
            _dataSyncService = dataSyncService;
        }

        public async Task SyncEverythingAsync()
        {
            await SyncMultiTenantAsync();
            await SyncMonoTenantAsync(new HashSet<string>());
        }

        public async Task SyncMonoTenantAsync(HashSet<string> subdomains)
        {
            var dataSyncStrategy = subdomains.Any()
                ? DataSyncStrategy.SyncSpecificEnvironmentsOnly
                : DataSyncStrategy.SyncEverything;

            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });

            await _dataSyncService.SyncTenantsDataAsync(environments, dataSyncStrategy);
        }

        public async Task SyncRandomMonoTenantAsync(int tenantCount)
        {
            var environments = (await _environmentsStore.GetAsync(new EnvironmentFilter()))
                .Shuffle()
                .Take(tenantCount)
                .ToList();

            await _dataSyncService.SyncTenantsDataAsync(environments, DataSyncStrategy.SyncSpecificEnvironmentsOnly);
        }

        public Task SyncMultiTenantAsync()
        {
            return _dataSyncService.SyncMultiTenantDataAsync();
        }

        public async Task PurgeEverythingAsync()
        {
            await _dataSyncService.PurgeEverythingAsync();
        }

        public async Task PurgeTenantsAsync(HashSet<string> subdomains)
        {
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });
            await _dataSyncService.PurgeTenantsDataAsync(environments);
        }
    }
}
