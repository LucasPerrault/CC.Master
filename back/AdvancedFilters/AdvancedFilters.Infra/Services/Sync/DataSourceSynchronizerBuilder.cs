using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Core;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using AdvancedFilters.Infra.Storage.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly ILocalDataSourceService _localDataSourceService;

        public DataSourceSyncCreationService
        (
            HttpClient httpClient,
            IBulkUpsertService bulk,
            FetchAuthenticator authenticator,
            ILogger<IDataSourceSynchronizer> logger,
            ILocalDataSourceService localDataSourceService
        )
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _authenticator = authenticator;
            _logger = logger;
            _localDataSourceService = localDataSourceService;
        }

        public IDataSourceSynchronizerBuilder ForEnvironments(List<Environment> environments, DataSyncStrategy strategy)
        {
            return new DataSourceSynchronizerBuilder(_httpClient, _bulk, _localDataSourceService, _authenticator, _logger, environments, strategy);
        }
    }

    public class DataSourceSynchronizerBuilder : IDataSourceSynchronizerBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly IBulkUpsertService _bulk;
        private readonly FetchAuthenticator _authenticator;
        private readonly ILogger<IDataSourceSynchronizer> _logger;
        private readonly ILocalDataSourceService _localDataSourceService;
        private readonly List<Environment> _environments;
        private readonly DataSyncStrategy _dataSyncStrategy;
        private readonly HashSet<int> _environmentIds;

        public DataSourceSynchronizerBuilder
        (
            HttpClient httpClient,
            IBulkUpsertService bulk,
            ILocalDataSourceService localDataSourceService,
            FetchAuthenticator authenticator,
            ILogger<IDataSourceSynchronizer> logger,
            List<Environment> environments,
            DataSyncStrategy dataSyncStrategy
        )
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _localDataSourceService = localDataSourceService;
            _authenticator = authenticator;
            _logger = logger;
            _environments = environments;
            _environmentIds = environments.Select(e => e.Id).ToHashSet();
            _dataSyncStrategy = dataSyncStrategy;

        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(CountryDataSource dataSource)
        {
            Func<Task<IReadOnlyCollection<Country>>> getCountriesAsync = () => _localDataSourceService.GetAllCountriesAsync();
            var config = new BulkUpsertConfig();
            return Task.FromResult(BuildFromLocal(getCountriesAsync, config));
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(EnvironmentDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<Environment>("CC.Master");
            var config = new BulkUpsertConfig();
            var synchronizer = BuildFrom<EnvironmentsDto, Environment, EmptyDataSourceContext<Environment>>(dataSource, new List<EmptyDataSourceContext<Environment>> { context }, config);
            return Task.FromResult(synchronizer);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(EstablishmentDataSource dataSource)
        {
            Action<Environment, Establishment> finalizeAction = (environment, establishment) => { establishment.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<Establishment>(c => c.EnvironmentId)
            };
            return BuildFrom<EstablishmentsDto, Establishment, EnvironmentDataSourceContext<Establishment>>(dataSource, contexts, config);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(AppInstanceDataSource dataSource)
        {
            Action<Environment, AppInstance> finalizeAction = (environment, appInstance) => { appInstance.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<AppInstance>(c => c.EnvironmentId)
            };
            return BuildFrom<AppInstancesDto, AppInstance, EnvironmentDataSourceContext<AppInstance>>(dataSource, contexts, config);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(LegalUnitDataSource dataSource)
        {
            Action<Environment, LegalUnit> finalizeAction = (environment, legalUnit) => { legalUnit.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<LegalUnit>(c => c.EnvironmentId)
            };
            return BuildFrom<LegalUnitsDto, LegalUnit, EnvironmentDataSourceContext<LegalUnit>>(dataSource, contexts, config);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(ContractDataSource dataSource)
        {
            var context = new SubdomainSubsetDataSourceContext<Contract>(_environments.Select(e => e.Subdomain).ToList(), dataSource.SubdomainsParamName, "CC.Master");
            var bulkUpsertConfig = new BulkUpsertConfig
            {
                IncludeSubEntities = true,
                Filter = GetFilter<Contract>(c => c.EnvironmentId)
            };
            var synchronizer = BuildFrom<ContractsDto, Contract, EmptyDataSourceContext<Contract>>(dataSource, new List<EmptyDataSourceContext<Contract>> { context }, config: bulkUpsertConfig);
            return Task.FromResult(synchronizer);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<Client>("CC.Master");
            var config = new BulkUpsertConfig();
            var synchronizer = BuildFrom<ClientsDto, Client, EmptyDataSourceContext<Client>>(dataSource, new List<EmptyDataSourceContext<Client>> { context }, config);
            return Task.FromResult(synchronizer);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource dataSource)
        {
            Action<Environment, AppContact> finalizeAction = (environment, contact) => { contact.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<AppContact>(c => c.EnvironmentId)
            };
            return BuildFrom<AppContactsDto, AppContact, EnvironmentDataSourceContext<AppContact>>(dataSource, contexts, config);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(ClientContactDataSource dataSource)
        {
            Action<Environment, ClientContact> finalizeAction = (environment, c) => { c.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<ClientContact>(c => c.EnvironmentId)
            };
            return BuildFrom<ClientContactsDto, ClientContact, EnvironmentDataSourceContext<ClientContact>>(dataSource, contexts, config);
        }

        public async Task<IDataSourceSynchronizer> BuildFromAsync(SpecializedContactDataSource dataSource)
        {
            Action<Environment, SpecializedContact> finalizeAction = (environment, c) => { c.EnvironmentId = environment.Id; };
            var contexts = await GetEnvironmentContextsAsync(finalizeAction);
            var config = new BulkUpsertConfig
            {
                Filter = GetFilter<SpecializedContact>(c => c.EnvironmentId)
            };
            return BuildFrom<SpecializedContactsDto, SpecializedContact, EnvironmentDataSourceContext<SpecializedContact>>(dataSource, contexts, config);
        }

        private UpsertFilter GetFilter<T>(Expression<Func<T, int>> getId) where T : class
        {
            return _dataSyncStrategy switch
            {
                DataSyncStrategy.SyncEverything => UpsertFilter.Everything(),
                DataSyncStrategy.SyncSpecificEnvironmentsOnly => UpsertFilter.ForEnvironments(_environmentIds, getId),
                _ => throw new ApplicationException($"Unsupported data sync strategy {_dataSyncStrategy}")
            };
        }

        private IDataSourceSynchronizer BuildFromLocal<T>(Func<Task<IReadOnlyCollection<T>>> getItemsAction, BulkUpsertConfig config) where T : class
        {
            return new LocalDataSourceSynchronizer<T>
            (
                getItemsAction,
                entities => _bulk.InsertOrUpdateOrDeleteAsync
                (
                    entities,
                    config
                )
            );
        }

        private IDataSourceSynchronizer BuildFrom<TDto, T, TContext>
        (
            RemoteDataSource dataSource,
            IReadOnlyCollection<TContext> contexts,
            BulkUpsertConfig config
        )
            where TDto : IDto<T> where T : class where TContext : IDataSourceContext<T>
        {
            var jobs = GetJobs<T, TContext>(dataSource, contexts).ToList();
            return FromJobs<TDto, T>(jobs, config);
        }

        private Task<List<EnvironmentDataSourceContext<T>>> GetEnvironmentContextsAsync<T>(Action<Environment, T> finalizeAction)
        {
            return Task.FromResult(_environments.Select(e => new EnvironmentDataSourceContext<T>(e, finalizeAction)).ToList());
        }

        private RemoteDataSourceSynchronizer<TDto, T> FromJobs<TDto, T>(List<FetchJob<T>> jobs, BulkUpsertConfig config) where TDto : IDto<T> where T : class
        {
            return new RemoteDataSourceSynchronizer<TDto, T>(jobs, _httpClient.SendAsync, entities => _bulk.InsertOrUpdateOrDeleteAsync(entities, config), _logger);
        }

        private class EmptyDataSourceContext<T> : IDataSourceContext<T>
        {
            private readonly string _targetCode;

            public string GetTargetCode()
            {
                return _targetCode;
            }

            public EmptyDataSourceContext(string targetCode)
            {
                _targetCode = targetCode;
            }

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

            public SubdomainSubsetDataSourceContext(IReadOnlyCollection<string> subdomains, string environmentsParamName, string targetCode) : base(targetCode)
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

            public string GetTargetCode()
            {
                return $"Environment:{Environment.Subdomain}";
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
                .Select(context => new FetchJob<T>(dataSource.DataSourceRoute.GetUri<T, TContext>(context), _authenticator.Authenticate(dataSource.Authentication), context.Finalize, context.GetTargetCode()))
                .ToList();
        }
    }
}
