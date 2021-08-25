using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Storage.Services;
using System.Net.Http;

namespace AdvancedFilters.Infra.Services
{
    public class DataSourceSynchronizerBuilder : IDataSourceSynchronizerBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly BulkUpsertService _bulk;

        public DataSourceSynchronizerBuilder(HttpClient httpClient, BulkUpsertService bulk)
        {
            _httpClient = httpClient;
            _bulk = bulk;
        }

        public IDataSourceSynchronizer BuildFrom(EnvironmentDataSource dataSource)
        {
            return new DataSourceSynchronizer<EnvironmentsDto, Environment>(_httpClient, _bulk, dataSource);
        }

        public IDataSourceSynchronizer BuildFrom(EstablishmentDataSource dataSource)
        {
            return new DataSourceSynchronizer<EstablishmentDto, Establishment>(_httpClient, _bulk, dataSource);
        }

        public IDataSourceSynchronizer BuildFrom(AppInstanceDataSource dataSource)
        {
            return new DataSourceSynchronizer<AppInstancesDto, AppInstance>(_httpClient, _bulk, dataSource);
        }

        public IDataSourceSynchronizer BuildFrom(LegalUnitDataSource dataSource)
        {
            return new DataSourceSynchronizer<LegalUnitDto, LegalUnit>(_httpClient, _bulk, dataSource);
        }
    }
}
