using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using Email.Domain;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class SyncService : ISyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSyncCreationService _creationService;
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly IEmailService _emailService;
        private readonly ISyncEmails _syncEmails;
        private readonly ITeamNotifier _teamNotifier;

        public SyncService
        (
            DataSourcesRepository dataSourcesRepository,
            IDataSourceSyncCreationService creationService,
            IEnvironmentsStore environmentsStore,
            IEmailService emailService,
            ISyncEmails syncEmails,
            ITeamNotifier teamNotifier
        )
        {
            _dataSourcesRepository = dataSourcesRepository;
            _creationService = creationService;
            _environmentsStore = environmentsStore;
            _emailService = emailService;
            _syncEmails = syncEmails;
            _teamNotifier = teamNotifier;
        }

        public async Task SyncEverythingAsync()
        {
            await SyncMultiTenantDataAsync();
            await SyncMonoTenantDataAsync(new HashSet<string>());
        }

        public async Task SyncMonoTenantDataAsync(HashSet<string> subdomains)
        {
            var dataSyncStrategy = subdomains.Any()
                ? DataSyncStrategy.SyncSpecificEnvironmentsOnly
                : DataSyncStrategy.SyncEverything;

            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });
            await SyncTenantsDataAsync(environments, dataSyncStrategy);
        }

        public async Task SyncRandomMonoTenantDataAsync(int tenantCount)
        {
            var environments = ( await _environmentsStore.GetAsync(new EnvironmentFilter()) )
                .Shuffle()
                .Take(tenantCount)
                .ToList();
            await SyncTenantsDataAsync(environments, DataSyncStrategy.SyncSpecificEnvironmentsOnly);
        }

        private async Task SyncTenantsDataAsync(List<Environment> environments, DataSyncStrategy strategy)
        {
            var builder = _creationService.ForEnvironments(environments, strategy);
            var dataSources = _dataSourcesRepository.GetMonoTenant();
            await SyncAsync(dataSources, builder);
        }

        public async Task SyncMultiTenantDataAsync()
        {
            var builder = _creationService.ForEnvironments(new List<Environment>(), DataSyncStrategy.SyncEverything);
            var dataSources = _dataSourcesRepository.GetMultiTenant();
            await SyncAsync(dataSources, builder);
        }

        public async Task PurgeEverythingAsync()
        {
            await _teamNotifier.NotifyAsync(Team.CafeAdmins, ":coffee: Cafe : complete data purge has been requested");
            var builder = _creationService.ForEnvironments(new List<Environment>(), DataSyncStrategy.SyncEverything);
            var dataSources = _dataSourcesRepository.GetAll();
            foreach (var dataSource in dataSources.Reverse())
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builder);
                await synchronizer.PurgeAsync();
            }
            await _teamNotifier.NotifyAsync(Team.CafeAdmins, ":coffee: Cafe : all data has been purged");
        }

        public async Task PurgeTenantsAsync(HashSet<string> subdomains)
        {
            var environments = await _environmentsStore.GetAsync(new EnvironmentFilter { Subdomains = subdomains });
            await _teamNotifier.NotifyAsync(Team.CafeAdmins, $":coffee: Cafe : purge of {environments.Count} tenants has been requested");
            var builder = _creationService.ForEnvironments(environments, DataSyncStrategy.SyncSpecificEnvironmentsOnly);
            var dataSources = _dataSourcesRepository.GetMonoTenant();
            foreach (var dataSource in dataSources.Reverse())
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builder);
                await synchronizer.PurgeAsync();
            }
            await _teamNotifier.NotifyAsync(Team.CafeAdmins, $":coffee: Cafe : {environments.Count} tenants have been purged");
        }

        private async Task SyncAsync(IEnumerable<DataSource> dataSources, IDataSourceSynchronizerBuilder builder)
        {
            var missedTargets = new HashSet<string>();
            var exceptions = new List<Exception>();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builder);
                var syncResult = await synchronizer.SyncAsync(missedTargets);
                foreach (var missedTarget in syncResult.MissedTargets)
                {
                    missedTargets.Add(missedTarget);
                }

                exceptions.AddRange(syncResult.Exceptions);
            }

            await NotifyAsync(exceptions);
        }

        private async Task NotifyAsync(List<Exception> exceptions)
        {
            if (!exceptions.Any())
            {
                return;
            }

            await _teamNotifier.NotifyAsync(Team.CafeAdmins, $":coffee: Cafe : sync encountered {exceptions.Count} errors");

            await _emailService.SendAsync
            (
                RecipientForm.FromContact(EmailContact.CloudControl),
                _syncEmails.GetSyncReportEmail(exceptions)
            );
        }
    }
}
