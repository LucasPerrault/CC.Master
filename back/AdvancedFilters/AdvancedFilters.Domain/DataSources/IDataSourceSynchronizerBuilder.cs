using AdvancedFilters.Domain.Instance;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSynchronizerBuilder
    {
        Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource configuration);
    }
}
