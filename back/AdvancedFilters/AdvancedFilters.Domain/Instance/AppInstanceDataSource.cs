using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Instance
{
    public class AppInstanceDataSource : RemoteDataSource
    {
        public AppInstanceDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override Task<IDataSourceSynchronizer> GetSynchronizerAsync(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFromAsync(this);
        }
    }
}
