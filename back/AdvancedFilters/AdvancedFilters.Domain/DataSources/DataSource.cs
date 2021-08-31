using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSources
    {
        Environments,
        Establishments,
        AppInstances,
        LegalUnits
    }

    public abstract class DataSource
    {
        public IDataSourceAuthentication Authentication { get; }
        public IDataSourceRoute DataSourceRoute { get; }

        protected DataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
        {
            Authentication = authentication;
            DataSourceRoute = dataSourceRoute;
        }

        public abstract Task<IDataSourceSynchronizer> GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder);
    }
}
