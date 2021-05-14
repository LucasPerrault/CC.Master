using Email.Domain;
using Instances.Application.Demos.Emails;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Instances.Application.Demos.Deletion
{
    public class InactiveDemosCleaner
    {
        private readonly ITimeProvider _timeProvider;
        private readonly IInstanceSessionLogsService _sessionLogsService;
        private readonly IDemosStore _demosStore;
        private readonly ICcDataService _ccDataService;
        private readonly IEmailService _emailService;
        private readonly IDemoEmails _demoEmails;
        private readonly IDemoDeletionCalculator _deletionCalculator;

        public InactiveDemosCleaner
        (
            ITimeProvider timeProvider,
            IInstanceSessionLogsService sessionLogsService,
            IDemosStore demosStore,
            ICcDataService ccDataService,
            IEmailService emailService,
            IDemoEmails demoEmails,
            IDemoDeletionCalculator deletionCalculator
        )
        {
            _timeProvider = timeProvider;
            _sessionLogsService = sessionLogsService;
            _demosStore = demosStore;
            _ccDataService = ccDataService;
            _emailService = emailService;
            _demoEmails = demoEmails;
            _deletionCalculator = deletionCalculator;
        }

        public async Task CleanAsync()
        {
            var eligibleDemos = new DemoFilter
            {
                IsActive = BoolCombination.TrueOnly,
                IsTemplate = BoolCombination.FalseOnly,
                IsProtected = BoolCombination.FalseOnly
            };
            var usages = await GetDemoUsagesAsync(eligibleDemos);
            await UpdateDemosAsync(usages);

            var today = _timeProvider.Today();
            var infos = await GetCleanupInfoAsync(usages, today);
            await ReportCleanupIntentionsAsync(infos);
            await DeleteAllAsync(infos.Where(i => i.State == DemoState.DeletionScheduledToday));
        }

        private async Task<DemoLastUsageRetrieveAttempt[]> GetDemoUsagesAsync(DemoFilter filter)
        {
            var activeDemos = await _demosStore.GetAsync(filter, AccessRight.All);
            var usagesTasks = activeDemos.Select(d => GetLastUsageAsync(d));
            return await Task.WhenAll(usagesTasks);
        }

        private async Task UpdateDemosAsync(IEnumerable<DemoLastUsageRetrieveAttempt> usages)
        {
            var usableUsages = usages
                .Where(a => a.Exception == null)
                .Where(a => a.LastUsedOn.HasValue)
                .Select(a => new DemoDeletionSchedule
                {
                    Demo = a.Demo,
                    DeletionScheduledOn = _deletionCalculator.GetDeletionDate(a.Demo, a.LastUsedOn.Value)
                }).ToList();

            await _demosStore.UpdateDeletionScheduleAsync(usableUsages);
        }

        private async Task DeleteAllAsync(IEnumerable<DemoCleanupInfo> info)
        {
            var clusterBatches = info.GroupBy(i => i.Demo.Instance.Cluster)
                .ToDictionary(group => group.Key, group => group.Select(i => i.Demo.Subdomain));

            foreach (var batch in clusterBatches)
            {
                await _ccDataService.DeleteInstancesAsync(batch.Value, batch.Key, $"/api/demos/deletion-report/{batch.Key}");
            }
        }

        private Task ReportCleanupIntentionsAsync(IEnumerable<DemoCleanupInfo> info)
        {
            return _emailService.SendAsync
            (
                new SenderForm { DisplayName = "Suppression des démos" },
                RecipientForm.FromContact(EmailContact.CloudControl),
                _demoEmails.GetIntentEmail(_timeProvider.Today(), info).Content
            );
        }

        private async Task<IEnumerable<DemoCleanupInfo>> GetCleanupInfoAsync
        (
            IEnumerable<DemoLastUsageRetrieveAttempt> attempts,
            DateTime today
        )
        {
            return attempts.Select
            (
                a => a.Exception != null
                    ? new DemoCleanupInfo(a.Demo, a.Exception, DemoState.ErrorAtStateEvaluation)
                    : new DemoCleanupInfo(a.Demo, today)
            );
        }

        private async Task<DemoLastUsageRetrieveAttempt> GetLastUsageAsync(Demo demo)
        {
            try
            {
                var latestConnection = await _sessionLogsService.GetLatestAsync(demo.Href);
                var lastUsedOn = latestConnection == new DateTime(0001, 1, 1)
                    ? demo.CreatedAt
                    : latestConnection;

                return new DemoLastUsageRetrieveAttempt { Demo = demo, LastUsedOn = lastUsedOn };
            }
            catch (Exception e)
            {
                return new DemoLastUsageRetrieveAttempt { Demo = demo, Exception = e};
            }
        }

        private class DemoLastUsageRetrieveAttempt
        {
            public Demo Demo { get; set; }
            public DateTime? LastUsedOn { get; set; }
            public Exception Exception { get; set; }
        }
    }
}
