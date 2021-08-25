using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Infra.Storage.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace AdvancedFilters.Infra.Services
{
    public class DataSourceSynchronizer<TDto, T> : IDataSourceSynchronizer
        where TDto : IDto<T>
        where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly BulkUpsertService _bulk;
        private readonly IDataSourceAuthentication _configurationAuthentication;
        private readonly IDataSourceRoute _configurationDataSourceRoute;

        public DataSourceSynchronizer(HttpClient httpClient, BulkUpsertService bulk, IDataSourceAuthentication configurationAuthentication, IDataSourceRoute configurationDataSourceRoute)
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _configurationAuthentication = configurationAuthentication;
            _configurationDataSourceRoute = configurationDataSourceRoute;
        }

        public async Task SyncAsync()
        {
            var requestUri = _configurationDataSourceRoute.RequestUri;
            using var response = await _httpClient.GetAsync(requestUri);
            using var stream = await response.Content.ReadAsStreamAsync();

            var dto = await Serializer.DeserializeAsync<TDto>(stream);
            var items = dto.ToItems();

            await _bulk.InsertOrUpdateOrDeleteAsync(items);
        }
    }
}
