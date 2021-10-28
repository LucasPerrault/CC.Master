using AdvancedFilters.Domain.DataSources;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Instance
{
    public class EnvironmentAccessDataSource : RemoteDataSource
    {
        public EnvironmentAccessDataSource(IDataSourceAuthentication authentication, IDataSourceRoute dataSourceRoute)
            : base(authentication, dataSourceRoute)
        { }

        public override Task<IDataSourceSynchronizer> GetSynchronizerAsync(IDataSourceSynchronizerBuilder synchronizerBuilder)
        {
            return synchronizerBuilder.BuildFromAsync(this);
        }
    }
}
