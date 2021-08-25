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

        public IDataSourceSynchronizer BuildFrom(EnvironmentDataSource configuration)
        {
            return new DataSourceSynchronizer<EnvironmentsDto, Environment>(_httpClient, _bulk, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(EstablishmentDataSource configuration)
        {
            return new DataSourceSynchronizer<EstablishmentDto, Establishment>(_httpClient, _bulk, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(AppInstanceDataSource configuration)
        {
            return new DataSourceSynchronizer<AppInstancesDto, AppInstance>(_httpClient, _bulk, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(LegalUnitDataSource configuration)
        {
            return new DataSourceSynchronizer<LegalUnitDto, LegalUnit>(_httpClient, _bulk, configuration.Authentication, configuration.DataSourceRoute);
        }
    }
}
