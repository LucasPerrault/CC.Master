using Email.Domain;
using Instances.Application.Demos.Emails;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using MoreLinq.Extensions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Instances.Application.Demos.Deletion
{
    public class DemoCleanupParams
    {
        public bool IsDryRun { get; set; }
    }

    public class InactiveDemosCleaner
    {
        private const int MaxConcurrentDemoUsageRetrievalAttempts = 200;
        private static readonly TimeSpan DelayBetweenConcurrentDemoUsageRetrievalAttempts = TimeSpan.FromMilliseconds(500);

        private readonly ITimeProvider _timeProvider;
        private readonly IInstanceSessionLogsService _sessionLogsService;
        private readonly IDemosStore _demosStore;
        private readonly ICcDataService _ccDataService;
        private readonly IEmailService _emailService;
        private readonly IDemoEmails _demoEmails;
        private readonly IDemoDeletionCalculator _deletionCalculator;
        private readonly IInstancesStore _instancesStore;
        private readonly IDnsService _dnsService;

        public InactiveDemosCleaner
        (
            ITimeProvider timeProvider,
            IInstanceSessionLogsService sessionLogsService,
            IDemosStore demosStore,
            ICcDataService ccDataService,
            IEmailService emailService,
            IDemoEmails demoEmails,
            IDemoDeletionCalculator deletionCalculator,
            IInstancesStore instancesStore,
            IDnsService dnsService
        )
        {
            _timeProvider = timeProvider;
            _sessionLogsService = sessionLogsService;
            _demosStore = demosStore;
            _ccDataService = ccDataService;
            _emailService = emailService;
            _demoEmails = demoEmails;
            _deletionCalculator = deletionCalculator;
            _instancesStore = instancesStore;
            _dnsService = dnsService;
        }

        public async Task CleanAsync(DemoCleanupParams demoCleanupParams)
        {
            var eligibleDemos = new DemoFilter
            {
                IsActive = CompareBoolean.TrueOnly,
                IsTemplate = CompareBoolean.FalseOnly,
                IsProtected = CompareBoolean.FalseOnly
            };
            var usages = await GetDemoUsagesAsync(eligibleDemos);
            await UpdateDemosAsync(usages);

            var today = _timeProvider.Today();
            var infos = GetCleanupInfo(usages, today);
            await ReportCleanupIntentionsAsync(infos);

            if (demoCleanupParams.IsDryRun)
            {
                return;
            }

            await DeleteAllAsync(infos.Where(i => i.State == DemoState.DeletionScheduledToday));
        }

        private async Task<List<DemoLastUsageRetrieveAttempt>> GetDemoUsagesAsync(DemoFilter filter)
        {
            var result = new List<DemoLastUsageRetrieveAttempt>();
            var activeDemos = await _demosStore.GetAsync(filter, AccessRight.All);

            foreach (var batch in activeDemos.Batch(MaxConcurrentDemoUsageRetrievalAttempts))
            {
                result.AddRange(batch.Select(a => new DemoLastUsageRetrieveAttempt
                {
                    Demo = a,
                    LastUsedOn = DateTime.Today.AddDays(-7),
                }));
            }
            return result;
        }

        private async Task<DemoLastUsageRetrieveAttempt[]> GetDemoUsagesWithoutHarassingPlatformAsync(IEnumerable<Demo> demos)
        {
            var delayTask = Task.Delay(DelayBetweenConcurrentDemoUsageRetrievalAttempts);
            var usagesTasks = demos.Select(d => GetLastUsageAsync(d));
            var usages = await Task.WhenAll(usagesTasks);
            await delayTask;
            return usages;
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
            var clusterBatches = info.GroupBy(i => i.Demo.Cluster)
                .ToDictionary(group => group.Key, group => group.Select(i => i.Demo.Subdomain));

            foreach (var batch in clusterBatches)
            {
                var entries = batch.Value.Select(s => DnsEntry.ForDemo(s, batch.Key));
                await _dnsService.DeleteAsync(entries);
                await _ccDataService.DeleteInstancesAsync(batch.Value, batch.Key, $"/api/demos/deletion-report/{batch.Key}");
            }

            await _demosStore.DeleteAsync(info.Select(i => i.Demo));
            await _instancesStore.DeleteForDemoAsync(info.Select(i => i.Demo.Instance));
        }

        private Task ReportCleanupIntentionsAsync(IEnumerable<DemoCleanupInfo> info)
        {
            return _emailService.SendAsync
            (
                RecipientForm.FromContact(EmailContact.CloudControl),
                _demoEmails.GetIntentEmail(_timeProvider.Today(), info).Content
            );
        }

        private IEnumerable<DemoCleanupInfo> GetCleanupInfo
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
