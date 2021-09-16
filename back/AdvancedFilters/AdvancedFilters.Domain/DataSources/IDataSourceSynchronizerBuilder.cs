using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Instance;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSynchronizerBuilder
    {
        IDataSourceSynchronizerBuilder WithFilter(SyncFilter filter);
        Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ContractDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientContactDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource dataSource);
    }
}
