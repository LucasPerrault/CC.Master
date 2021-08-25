using AdvancedFilters.Domain.DataSources;

namespace AdvancedFilters.Domain.Instance
{
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
}
