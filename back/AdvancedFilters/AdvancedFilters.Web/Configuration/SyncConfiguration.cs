using AdvancedFilters.Domain;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;
using Environment = AdvancedFilters.Domain.Environment;

namespace AdvancedFilters.Web.Configuration
{
    public class DataSourceSynchronizer<TDto, T> : IDataSourceSynchronizer where TDto : IDto<T>
    {
        private readonly HttpClient _httpClient;
        private readonly IDataSourceAuthentication _configurationAuthentication;
        private readonly IDataSourceRoute _configurationDataSourceRoute;

        public DataSourceSynchronizer(HttpClient httpClient, IDataSourceAuthentication configurationAuthentication, IDataSourceRoute configurationDataSourceRoute)
        {
            _httpClient = httpClient;
            _configurationAuthentication = configurationAuthentication;
            _configurationDataSourceRoute = configurationDataSourceRoute;
        }

        public async Task SyncAsync()
        {
            var response = await _httpClient.GetAsync("");
            var stream = await response.Content.ReadAsStreamAsync();
            var dto = await Serializer.DeserializeAsync<TDto>(stream);
            var items = dto.ToItems();
        }
    }

    public class DataSourceSynchronizerBuilder : IDataSourceSynchronizerBuilder
    {
        private readonly HttpClient _httpClient;

        public DataSourceSynchronizerBuilder(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IDataSourceSynchronizer BuildFrom(EnvironmentDataSource configuration)
        {
            return new DataSourceSynchronizer<EnvironmentsDto, Environment>(_httpClient, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(EstablishmentDataSource configuration)
        {
            return new DataSourceSynchronizer<EstablishmentDto, Establishment>(_httpClient, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(AppInstanceDataSource configuration)
        {
            return new DataSourceSynchronizer<AppInstancesDto, AppInstance>(_httpClient, configuration.Authentication, configuration.DataSourceRoute);
        }

        public IDataSourceSynchronizer BuildFrom(LegalUnitDataSource configuration)
        {
            return new DataSourceSynchronizer<LegalUnitDto, LegalUnit>(_httpClient, configuration.Authentication, configuration.DataSourceRoute);
        }
    }

    public interface IDto<T>
    {
        List<T> ToItems();
    }

    class EnvironmentsDto : IDto<Environment>
    {
        public List<Environment> Items { get; set; }

        public List<Environment> ToItems()
        {
            return Items;
        }
    }

    class EstablishmentDto : IDto<Establishment>
    {
        public List<Establishment> Items { get; set; }

        public List<Establishment> ToItems()
        {
            return Items;
        }
    }

    class AppInstancesDto : IDto<AppInstance>
    {
        public List<AppInstance> Items { get; set; }

        public List<AppInstance> ToItems()
        {
            return Items;
        }
    }

    class LegalUnitDto : IDto<LegalUnit>
    {
        public List<LegalUnit> Items { get; set; }

        public List<LegalUnit> ToItems()
        {
            return Items;
        }
    }

    public class CafeDbContext
    { }
}
