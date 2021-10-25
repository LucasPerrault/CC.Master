using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Contacts
{
    public class ClientContactDataSource : RemoteDataSource
    {
        public ClientContactDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override Task<IDataSourceSynchronizer> GetSynchronizerAsync(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFromAsync(this);
        }
    }
}
