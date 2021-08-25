using AdvancedFilters.Domain.DataSources;

namespace AdvancedFilters.Domain.Instance
{
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
}
