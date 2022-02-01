using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Core;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Domain.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceSyncCreationService
    {
        IDataSourceSynchronizerBuilder ForEnvironments(List<Environment> environments, SyncStrategy strategy);
    }

    public interface IDataSourceSynchronizerBuilder
    {
        Task<IDataSourceSynchronizer> BuildFromAsync(CountryDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ContractDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(ClientContactDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(DistributorDataSource distributorDataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource dataSource);
        Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentAccessDataSource environmentAccessDataSource);
    }
}
