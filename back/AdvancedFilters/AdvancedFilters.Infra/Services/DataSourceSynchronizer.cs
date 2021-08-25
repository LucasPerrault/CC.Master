using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Infra.Storage.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly DataSource _dataSource;

        public DataSourceSynchronizer(HttpClient httpClient, BulkUpsertService bulk, DataSource dataSource)
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _dataSource = dataSource;
        }

        public async Task SyncAsync()
        {
            var requestUri = GetRequestUri();
            using var requestMsg = new HttpRequestMessage(HttpMethod.Get, requestUri);

            Authenticate(requestMsg);

            using var response = await _httpClient.SendAsync(requestMsg);
            using var stream = await response.Content.ReadAsStreamAsync();

            var dto = await Serializer.DeserializeAsync<TDto>(stream);
            var items = dto.ToItems();

            await _bulk.InsertOrUpdateOrDeleteAsync(items);
        }

        private string GetRequestUri()
        {
            var route = _dataSource.DataSourceRoute;
            switch (route)
            {
                case TenantDataSourceRoute tenantRoute:
                    return tenantRoute.Endpoint;
                case HostDataSourceRoute hostRoute:
                    return hostRoute.Host;
                default:
                    throw new ApplicationException($"DataSourceRoute { route.GetType() } not supported");
            }
        }

        private void Authenticate(HttpRequestMessage msg)
        {
            var auth = _dataSource.Authentication;
            switch (auth)
            {
                case AuthorizationAuthentication authorizationAuth:
                    Authenticate(msg, authorizationAuth);
                    break;
                default:
                    throw new ApplicationException($"Authentication type { auth.GetType() } not supported");
            }
        }

        private void Authenticate(HttpRequestMessage msg, AuthorizationAuthentication authAuth)
        {
            msg.Headers.Authorization = new AuthenticationHeaderValue(authAuth.Scheme, authAuth.Parameter);
        }
    }
}
