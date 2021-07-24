using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Domain
{
    public class DataSourceRepository
    {
        private readonly IReadOnlyDictionary<DataSources, DataSource> _sources;

        public DataSourceRepository(IReadOnlyDictionary<DataSources, DataSource> sources)
        {
            _sources = sources;
        }

        public DataSource Get(DataSources source) => _sources[source];

        public List<DataSource> Get(params DataSources[] sources) => _sources
            .Where(s => sources.Contains(s.Key))
            .Select(s => s.Value)
            .ToList();

        public List<DataSource> GetAll() => _sources.Select(entry => entry.Value).ToList();
    }

    public enum DataSources
    {
        Environments,
        Establishments
    }

    public class DataSource
    {
        public DataSources Source { get; set; }
        public IDataSourceRoute Route { get; set; }
    }
}
