using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using Email.Domain;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class SyncService
    {
        private readonly DataSourcesRepository _dataSourcesRepository;
        private readonly IDataSourceSyncCreationService _creationService;
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly IEmailService _emailService;
        private readonly ISyncEmails _syncEmails;

        public SyncService
        (
            DataSourcesRepository dataSourcesRepository,
            IDataSourceSyncCreationService creationService,
            IEnvironmentsStore environmentsStore,
            IEmailService emailService,
            ISyncEmails syncEmails
        )
        {
            _dataSourcesRepository = dataSourcesRepository;
            _creationService = creationService;
            _environmentsStore = environmentsStore;
            _emailService = emailService;
            _syncEmails = syncEmails;
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
            var builderWithFilter = _creationService.ForEnvironments(environments, strategy);
            var dataSources = _dataSourcesRepository.GetMonoTenant();
            await SyncAsync(dataSources, builderWithFilter);
        }

        public async Task SyncMultiTenantDataAsync()
        {
            var builderWithFilter = _creationService.ForEnvironments(new List<Environment>(), DataSyncStrategy.SyncEverything);
            var dataSources = _dataSourcesRepository.GetMultiTenant();
            await SyncAsync(dataSources, builderWithFilter);
        }

        private async Task SyncAsync(IEnumerable<DataSource> dataSources, IDataSourceSynchronizerBuilder builderWithFilter)
        {
            var missedTargets = new HashSet<string>();
            var exceptions = new List<Exception>();
            foreach (var dataSource in dataSources)
            {
                var synchronizer = await dataSource.GetSynchronizerAsync(builderWithFilter);
                var syncResult = await synchronizer.SyncAsync(missedTargets);
                foreach (var missedTarget in syncResult.MissedTargets)
                {
                    missedTargets.Add(missedTarget);
                }

                exceptions.AddRange(syncResult.Exceptions);
            }

            await NotifyAsync(exceptions);
        }

        private Task NotifyAsync(List<Exception> exceptions)
        {
            if (!exceptions.Any())
            {
                return Task.CompletedTask;
            }

            return _emailService.SendAsync
            (
                new SenderForm { DisplayName = "Cafe Sync - Rapport" },
                RecipientForm.FromContact(EmailContact.CloudControl),
                _syncEmails.GetSyncReportEmail(exceptions).Content
            );
        }
    }
}
