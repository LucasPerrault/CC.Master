using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Domain.DataSources
{
    public class DataSourcesRepository
    {
        private readonly Dictionary<DataSources, DataSource> _dataSources;

        private static readonly IReadOnlyCollection<DataSources> MultiTenantSource = new List<DataSources>
        {
            DataSources.Environments,
            DataSources.Clients
        };

        public DataSourcesRepository(Dictionary<DataSources, DataSource> dataSources)
        {
            _dataSources = dataSources;
        }

        public DataSource Get(DataSources dataSource) => _dataSources[dataSource];
        public IEnumerable<DataSource> GetAll() => _dataSources.Select(kvp => kvp.Value);

        public IEnumerable<DataSource> GetMonoTenant() => _dataSources
            .Where(kvp => !MultiTenantSource.Contains(kvp.Key))
            .Select(kvp => kvp.Value);

        public IEnumerable<DataSource> GetMultiTenant() => _dataSources
            .Where(kvp => MultiTenantSource.Contains(kvp.Key))
            .Select(kvp => kvp.Value);
    }
}
