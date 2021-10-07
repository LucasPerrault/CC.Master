using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Core;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSyncStrategy
    {
        SyncEverything = 0,
        SyncSpecificEnvironmentsOnly = 1
    }

    public interface IDataSourceSyncCreationService
    {
        IDataSourceSynchronizerBuilder ForEnvironments(List<Environment> environments, DataSyncStrategy strategy);
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
        Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource dataSource);
    }
}
