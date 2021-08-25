using AdvancedFilters.Domain.DataSources;

namespace AdvancedFilters.Domain.Instance
{
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
}
