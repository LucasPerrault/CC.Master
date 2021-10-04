using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Domain.DataSources
{
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
}
