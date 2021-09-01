using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts
{
    public class SpecializedContactDataSource : DataSource
    {
        public SpecializedContactDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override Task<IDataSourceSynchronizer> GetSynchronizer(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFromAsync(this);
        }
    }
}
