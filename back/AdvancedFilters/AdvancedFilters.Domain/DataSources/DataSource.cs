using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSources
    {
        Environments,
        AppInstances,
        LegalUnits,
        Establishments,
        Clients,
        Contracts,
        AppContacts,
        ClientContacts,
        SpecializedContacts
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

        public abstract Task<IDataSourceSynchronizer> GetSynchronizerAsync(IDataSourceSynchronizerBuilder synchronizerBuilder);
    }
}
