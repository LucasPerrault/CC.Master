using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Billing;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSynchronizerBuilder
    {
        Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(ContractDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource configuration);
    }
}
