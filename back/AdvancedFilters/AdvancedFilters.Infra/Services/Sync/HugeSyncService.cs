using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class HugeSyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSyncCreationService _creationService;

        public HugeSyncService(DataSourcesRepository dataSourcesRepository, IDataSourceSyncCreationService creationService)
        {
            _dataSourcesRepository = dataSourcesRepository;
            _creationService = creationService;
        }

        public async Task SyncAsync(SyncFilter filter)
        {
            var builderWithFilter = _creationService.WithFilter(filter);

            var dataSources = _dataSourcesRepository.GetAll();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                await synchronizer.SyncAsync();
            }
        }
    }
}
