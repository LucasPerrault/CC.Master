using AdvancedFilters.Domain;
using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Web
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

            foreach (var syncConf in _dataSourcesRepository.GetAll())
            {
                await syncConf.GetSynchronizer(_builder).SyncAsync();
            }
        }
    }
}
