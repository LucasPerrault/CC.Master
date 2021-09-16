using AdvancedFilters.Domain.DataSources;
using System.Collections.Generic;
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

        public async Task SyncAsync(SyncFilter filter)
        {
            var builderWithFilter = _builder.WithFilter(filter);

            var dataSources = _dataSourcesRepository.GetAll();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                await synchronizer.SyncAsync();
            }
        }
    }
}
