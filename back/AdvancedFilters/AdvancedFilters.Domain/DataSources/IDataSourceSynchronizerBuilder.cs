using AdvancedFilters.Domain.Instance;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSynchronizerBuilder
    {
        IDataSourceSynchronizer BuildFrom(EnvironmentDataSource configuration);
        IDataSourceSynchronizer BuildFrom(EstablishmentDataSource configuration);
        IDataSourceSynchronizer BuildFrom(AppInstanceDataSource configuration);
        IDataSourceSynchronizer BuildFrom(LegalUnitDataSource configuration);
    }
}
