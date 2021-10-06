using AdvancedFilters.Domain.DataSources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class SyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSyncCreationService _creationService;

        public SyncService(DataSourcesRepository dataSourcesRepository, IDataSourceSyncCreationService creationService)
        {
            _dataSourcesRepository = dataSourcesRepository;
            _creationService = creationService;
        }

        public async Task SyncAsync(SyncFilter filter)
        {
            var builderWithFilter = _creationService.WithFilter(filter);

            var dataSources = GetDataSources(filter.SyncMode);
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                await synchronizer.SyncAsync();
            }
        }

        private IEnumerable<DataSource> GetDataSources(DataSourceSyncMode syncMode)
        {
            return syncMode switch
            {
                DataSourceSyncMode.Everything => _dataSourcesRepository.GetAll(),
                DataSourceSyncMode.MonoTenant => _dataSourcesRepository.GetMonoTenant(),
                DataSourceSyncMode.MultiTenant => _dataSourcesRepository.GetMultiTenant(),
            };
        }
    }
}
