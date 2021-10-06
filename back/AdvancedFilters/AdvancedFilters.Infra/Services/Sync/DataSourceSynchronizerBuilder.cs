using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Core;
using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using AdvancedFilters.Infra.Storage.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class DataSourceSyncCreationService : IDataSourceSyncCreationService
    {

        private readonly HttpClient _httpClient;
        private readonly IBulkUpsertService _bulk;
        private readonly FetchAuthenticator _authenticator;
        private readonly ILogger<IDataSourceSynchronizer> _logger;
        private readonly IEnvironmentsStore _store;
        private readonly ICountriesCollection _countriesCollection;

        public DataSourceSyncCreationService
        (
            HttpClient httpClient,
            IBulkUpsertService bulk,
            FetchAuthenticator authenticator,
            IEnvironmentsStore store,
            ICountriesCollection countriesCollection,
            ILogger<IDataSourceSynchronizer> logger
        )
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _authenticator = authenticator;
            _store = store;
            _countriesCollection = countriesCollection;
            _logger = logger;
        }

        public IDataSourceSynchronizerBuilder WithFilter(SyncFilter filter)
        {
            return new DataSourceSynchronizerBuilder(_httpClient, _bulk, _store, _countriesCollection, _authenticator, _logger, filter);
        }
    }

    public class DataSourceSynchronizerBuilder : IDataSourceSynchronizerBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly IBulkUpsertService _bulk;
        private readonly FetchAuthenticator _authenticator;
        private readonly ILogger<IDataSourceSynchronizer> _logger;
        private readonly IEnvironmentsStore _store;
        private readonly ICountriesCollection _countriesCollection;
        private readonly SyncFilter _filter;


        public DataSourceSynchronizerBuilder
        (
            HttpClient httpClient,
            IBulkUpsertService bulk,
            IEnvironmentsStore store,
            ICountriesCollection countriesCollection,
            FetchAuthenticator authenticator,
            ILogger<IDataSourceSynchronizer> logger,
            SyncFilter syncFilter
        )
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _store = store;
            _countriesCollection = countriesCollection;
            _authenticator = authenticator;
            _logger = logger;
            _filter = syncFilter;
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(CountryDataSource dataSource)
        {
            Func<Task<List<Country>>> getCountriesAsync = () => _countriesCollection.GetAllAsync();
            return Task.FromResult(BuildFromLocal(getCountriesAsync));
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<Environment>();
            var synchronizer = BuildFrom<EnvironmentsDto, Environment, EmptyDataSourceContext<Environment>>(dataSource, new List<EmptyDataSourceContext<Environment>> { context });
            return Task.FromResult(synchronizer);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource dataSource)
        {
            Action<Environment, Establishment> finalizeAction = (environment, establishment) => { establishment.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<EstablishmentsDto, Establishment, EnvironmentDataSourceContext<Establishment>>(dataSource, contexts);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource dataSource)
        {
            Action<Environment, AppInstance> finalizeAction = (environment, appInstance) => { appInstance.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<AppInstancesDto, AppInstance, EnvironmentDataSourceContext<AppInstance>>(dataSource, contexts);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource dataSource)
        {
            Action<Environment, LegalUnit> finalizeAction = (environment, legalUnit) => { legalUnit.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<LegalUnitsDto, LegalUnit, EnvironmentDataSourceContext<LegalUnit>>(dataSource, contexts);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(ContractDataSource dataSource)
        {
            var context = new SubdomainSubsetDataSourceContext<Contract>(_filter.Subdomains, dataSource.SubdomainsParamName);
            var bulkUpsertConfig = new BulkUpsertConfig
            {
                IncludeSubEntities = true
            };
            var synchronizer = BuildFrom<ContractsDto, Contract, EmptyDataSourceContext<Contract>>(dataSource, new List<EmptyDataSourceContext<Contract>> { context }, config: bulkUpsertConfig);
            return Task.FromResult(synchronizer);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<Client>();
            var synchronizer = BuildFrom<ClientsDto, Client, EmptyDataSourceContext<Client>>(dataSource, new List<EmptyDataSourceContext<Client>> { context });
            return Task.FromResult(synchronizer);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource dataSource)
        {
            Action<Environment, AppContact> finalizeAction = (environment, contact) => { contact.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<AppContactsDto, AppContact, EnvironmentDataSourceContext<AppContact>>(dataSource, contexts);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(ClientContactDataSource dataSource)
        {
            Action<Environment, ClientContact> finalizeAction = (environment, c) => { c.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<ClientContactsDto, ClientContact, EnvironmentDataSourceContext<ClientContact>>(dataSource, contexts);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource dataSource)
        {
            Action<Environment, SpecializedContact> finalizeAction = (environment, c) => { c.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            return BuildFrom<SpecializedContactsDto, SpecializedContact, EnvironmentDataSourceContext<SpecializedContact>>(dataSource, contexts);
        }

        private IDataSourceSynchronizer BuildFromLocal<T>(Func<Task<List<T>>> getItemsAction, BulkUpsertConfig config = null) where T : class
        {
            return new LocalDataSourceSynchronizer<T>
            (
                getItemsAction,
                entities => _bulk.InsertOrUpdateOrDeleteAsync
                (
                    entities,
                    config ?? new BulkUpsertConfig()
                )
            );
        }

        private IDataSourceSynchronizer BuildFrom<TDto, T, TContext>
        (
            RemoteDataSource dataSource,
            IReadOnlyCollection<TContext> contexts,
            BulkUpsertConfig config = null
        )
            where TDto : IDto<T> where T : class where TContext : IDataSourceContext<T>
        {
            var jobs = GetJobs<T, TContext>(dataSource, contexts).ToList();
            return FromJobs<TDto, T>(jobs, config ?? new BulkUpsertConfig());
        }

        private async Task<List<EnvironmentDataSourceContext<T>>> GetEnvironmentContextsAsync<T>(Action<Environment, T> finalizeAction)
        {
            var envFilter = new EnvironmentFilter { Subdomains = _filter.Subdomains };

            var envs = await _store.GetAsync(envFilter);
            return envs.Select(e => new EnvironmentDataSourceContext<T>(e, finalizeAction)).ToList();
        }

        private RemoteDataSourceSynchronizer<TDto, T> FromJobs<TDto, T>(List<FetchJob<T>> jobs, BulkUpsertConfig config) where TDto : IDto<T> where T : class
        {
            return new RemoteDataSourceSynchronizer<TDto, T>(jobs, _httpClient.SendAsync, entities => _bulk.InsertOrUpdateOrDeleteAsync(entities, config), _logger);
        }

        private class EmptyDataSourceContext<T> : IDataSourceContext<T>
        {
            public void Finalize(T item)
            { }

            public Uri GetUri(TenantDataSourceRoute route)
            {
                throw new NotImplementedException();
            }

            public virtual Uri GetUri(HostDataSourceRoute hostDataSourceRoute)
            {
                return hostDataSourceRoute.Uri;
            }
        }

        private sealed class SubdomainSubsetDataSourceContext<T> : EmptyDataSourceContext<T>
        {
            private readonly IReadOnlyCollection<string> _subdomains;
            private readonly string _subdomainsParamName;

            public SubdomainSubsetDataSourceContext(IReadOnlyCollection<string> subdomains, string environmentsParamName)
            {
                _subdomains = subdomains;
                _subdomainsParamName = environmentsParamName;
            }

            public override Uri GetUri(HostDataSourceRoute hostDataSourceRoute)
            {
                if (!_subdomains?.Any() ?? false)
                {
                    return hostDataSourceRoute.Uri;
                }

                var uriBuilder = new UriBuilder(hostDataSourceRoute.Uri);
                var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);

                var environmentsParam = string.Join(",", _subdomains);
                paramValues.Add(_subdomainsParamName, environmentsParam);

                uriBuilder.Query = paramValues.ToString();

                return uriBuilder.Uri;
            }
        }

        private sealed class EnvironmentDataSourceContext<T> : IDataSourceContext<T>
        {
            private readonly Action<Environment, T> _finalizeAction;
            public Environment Environment { get; }

            public EnvironmentDataSourceContext(Environment environment, Action<Environment, T> finalizeAction)
            {
                _finalizeAction = finalizeAction;
                Environment = environment;
            }

            public Uri GetUri(TenantDataSourceRoute route)
            {
                return new Uri(new Uri(Environment.ProductionHost), route.Endpoint);
            }

            public Uri GetUri(HostDataSourceRoute hostDataSourceRoute)
            {
                return hostDataSourceRoute.Uri;
            }

            public void Finalize(T item)
            {
                _finalizeAction(Environment, item);
            }
        }

        private IEnumerable<FetchJob<T>> GetJobs<T, TContext>(RemoteDataSource dataSource, IReadOnlyCollection<TContext> contexts)
            where TContext : IDataSourceContext<T>
        {
            return contexts
                .Select(context => new FetchJob<T>(dataSource.DataSourceRoute.GetUri<T, TContext>(context), _authenticator.Authenticate(dataSource.Authentication), context.Finalize))
                .ToList();
        }
    }
}
