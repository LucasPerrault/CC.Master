using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class SyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSyncCreationService _creationService;
        private readonly IEnvironmentsStore _environmentsStore;

        public SyncService(DataSourcesRepository dataSourcesRepository, IDataSourceSyncCreationService creationService, IEnvironmentsStore environmentsStore)
        {
            _dataSourcesRepository = dataSourcesRepository;
            _creationService = creationService;
            _environmentsStore = environmentsStore;
        }

        public async Task SyncEverythingAsync()
        {
            await SyncMultiTenantDataAsync();
            await SyncMonoTenantDataAsync(new HashSet<string>());
        }

        public async Task SyncMonoTenantDataAsync(HashSet<string> subdomains)
        {
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });
            var builderWithFilter = _creationService.ForEnvironments(environments);

            var dataSources = _dataSourcesRepository.GetMonoTenant();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                await synchronizer.SyncAsync();
            }
        }

        public async Task SyncMultiTenantDataAsync()
        {
            var builderWithFilter = _creationService.ForEnvironments(new List<Environment>());
            var dataSources = _dataSourcesRepository.GetMultiTenant();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                await synchronizer.SyncAsync();
            }
        }
    }
}
