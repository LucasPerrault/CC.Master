using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Instance;
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
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientContactDataSource configuration);
        Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource configuration);
    }
}
