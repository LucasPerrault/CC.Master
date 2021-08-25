using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSources
    {
        Environments,
        Establishments,
        AppInstances,
        LegalUnit
    }

    public class DataSourcesRepository
    {
        private readonly Dictionary<DataSources, DataSource> _dataSources;

        public DataSourcesRepository(Dictionary<DataSources, DataSource> dataSources)
        {
            _dataSources = dataSources;
        }

        public DataSource Get(DataSources dataSource) => _dataSources[dataSource];
        public IEnumerable<DataSource> GetAll() => _dataSources.Select(kvp => kvp.Value);
    }

    public abstract class DataSource
    {
        public IDataSourceAuthentication Authentication { get; }
        public IDataSourceRoute DataSourceRoute { get; }

        public DataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
        {
            Authentication = authentication;
            DataSourceRoute = dataSourceRoute;
        }

        public abstract IDataSourceSynchronizer GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder);
    }

    public interface IDataSourceSynchronizer
    {
        Task SyncAsync();
    }
}
