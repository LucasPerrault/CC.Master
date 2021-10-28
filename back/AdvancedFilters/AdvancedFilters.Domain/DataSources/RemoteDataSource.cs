using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSources
    {
        Countries,
        Environments,
        AppInstances,
        LegalUnits,
        Establishments,
        Clients,
        Distributors,
        AppContacts,
        ClientContacts,
        SpecializedContacts
    }

    public abstract class DataSource
    {
        public abstract Task<IDataSourceSynchronizer> GetSynchronizerAsync(IDataSourceSynchronizerBuilder synchronizerBuilder);
    }

    public abstract class LocalDataSource : DataSource
    { }

    public abstract class RemoteDataSource : DataSource
    {
        public IDataSourceAuthentication Authentication { get; }
        public IDataSourceRoute DataSourceRoute { get; }

        protected RemoteDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
        {
            Authentication = authentication;
            DataSourceRoute = dataSourceRoute;
        }
    }
}
