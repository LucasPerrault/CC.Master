using AdvancedFilters.Domain.Billing;
using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using AdvancedFilters.Infra.Storage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class DataSourceSynchronizerBuilder : IDataSourceSynchronizerBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly BulkUpsertService _bulk;
        private readonly FetchAuthenticator _authenticator;
        private readonly IEnvironmentsStore _store;

        public DataSourceSynchronizerBuilder
        (
            HttpClient httpClient,
            BulkUpsertService bulk,
            IEnvironmentsStore store,
            FetchAuthenticator authenticator
        )
        {
            _httpClient = httpClient;
            _bulk = bulk;
            _store = store;
            _authenticator = authenticator;
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
            var context = new EmptyDataSourceContext<Contract>();
            var synchronizer = BuildFrom<ContractsDto, Contract, EmptyDataSourceContext<Contract>>(dataSource, new List<EmptyDataSourceContext<Contract>> { context });
            return Task.FromResult(synchronizer);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(ClientDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<Client>();
            var synchronizer = BuildFrom<ClientsDto, Client, EmptyDataSourceContext<Client>>(dataSource, new List<EmptyDataSourceContext<Client>> { context });
            return Task.FromResult(synchronizer);
        }

        public Task<IDataSourceSynchronizer> BuildFromAsync(AppContactDataSource dataSource)
        {
            var context = new EmptyDataSourceContext<AppContact>();
            var synchronizer = BuildFrom<AppContactsDto, AppContact, EmptyDataSourceContext<AppContact>>(dataSource, new List<EmptyDataSourceContext<AppContact>> { context });
            return Task.FromResult(synchronizer);
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

        private IDataSourceSynchronizer BuildFrom<TDto, T, TContext>
            (DataSource dataSource, IReadOnlyCollection<TContext> contexts)
            where TDto : IDto<T> where T : class where TContext : IDataSourceContext<T>
        {
            var jobs = GetJobs<T, TContext>(dataSource, contexts).ToList();
            return FromJobs<TDto, T>(jobs);
        }

        private async Task<List<EnvironmentDataSourceContext<T>>> GetEnvironmentContextsAsync<T>(Action<Environment, T> finalizeAction)
        {
            var envs = await _store.GetAsync(new EnvironmentFilter());
            return envs.Select(e => new EnvironmentDataSourceContext<T>(e, finalizeAction)).ToList();
        }

        private DataSourceSynchronizer<TDto, T> FromJobs<TDto, T>(List<FetchJob<T>> jobs) where TDto : IDto<T> where T : class
        {
            return new DataSourceSynchronizer<TDto, T>(jobs, _httpClient.SendAsync, _bulk.InsertOrUpdateOrDeleteAsync);
        }


        private sealed class EmptyDataSourceContext<T> : IDataSourceContext<T>
        {
            public void Finalize(T item)
            { }

            public Uri GetUri(TenantDataSourceRoute route)
            {
                throw new NotImplementedException();
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

            public void Finalize(T item)
            {
                _finalizeAction(Environment, item);
            }
        }

        private IEnumerable<FetchJob<T>> GetJobs<T, TContext>(DataSource dataSource, IReadOnlyCollection<TContext> contexts) where TContext : IDataSourceContext<T>
        {
            return contexts
                .Select(p => new FetchJob<T>(dataSource.DataSourceRoute.GetUri<T, TContext>(p), _authenticator.Authenticate(dataSource.Authentication), p.Finalize))
                .ToList();
        }
    }
}
