using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain
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

    public interface IDataSourceSynchronizerBuilder
    {
        IDataSourceSynchronizer BuildFrom(EnvironmentDataSource configuration);
        IDataSourceSynchronizer BuildFrom(EstablishmentDataSource configuration);
        IDataSourceSynchronizer BuildFrom(AppInstanceDataSource configuration);
        IDataSourceSynchronizer BuildFrom(LegalUnitDataSource configuration);
    }

    public interface IDataSourceSynchronizer
    {
        Task SyncAsync();
    }

    public class EnvironmentDataSource : DataSource
    {
        public override IDataSourceSynchronizer GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFrom(this);
        }

        public EnvironmentDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }
    }


    public class EstablishmentDataSource : DataSource
    {
        public EstablishmentDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override IDataSourceSynchronizer GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFrom(this);
        }
    }

    public class LegalUnitDataSource : DataSource
    {
        public LegalUnitDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override IDataSourceSynchronizer GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFrom(this);
        }
    }

    public class AppInstanceDataSource : DataSource
    {
        public AppInstanceDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override IDataSourceSynchronizer GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFrom(this);
        }
    }
}
