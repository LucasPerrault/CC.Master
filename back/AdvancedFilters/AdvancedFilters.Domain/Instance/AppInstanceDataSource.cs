using AdvancedFilters.Domain.DataSources;

namespace AdvancedFilters.Domain.Instance
{
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
