using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class HugeSyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSynchronizerBuilder _builder;

        public HugeSyncService(DataSourcesRepository dataSourcesRepository, IDataSourceSynchronizerBuilder builder)
        {
            _dataSourcesRepository = dataSourcesRepository;
            _builder = builder;
        }

        public async Task SyncAsync()
        {
            var dataSources = _dataSourcesRepository.GetAll();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizer(_builder);
                await synchronizer.SyncAsync();
            }
        }
    }
}
