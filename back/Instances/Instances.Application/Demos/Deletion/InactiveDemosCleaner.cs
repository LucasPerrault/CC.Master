using Email.Domain;
using Instances.Application.Demos.Emails;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
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
            var today = _timeProvider.Today();
            var filter = new DemoFilter
            {
                IsActive = BoolCombination.TrueOnly,
                IsTemplate = BoolCombination.FalseOnly,
                IsProtected = BoolCombination.FalseOnly
            };

            var activeDemos = await _demosStore.GetAsync(filter, DemoAccess.All);
            var infoTasks = activeDemos
                .Select(d => GetUpdatedCleanupInfoAsync(d, today));

            var infos = await Task.WhenAll(infoTasks);

            await ReportCleanupIntentionsAsync(infos.Where(i => i.ShouldBeReported()));
            await DeleteAllAsync(infos.Where(i => i.State == DemoState.DeletionScheduledToday));
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

        private async Task<DemoCleanupInfo> GetUpdatedCleanupInfoAsync(Demo demo, DateTime demoDeletionSchedule)
        {
            try
            {
                await UpdateDeletionScheduleAsync(demo);
                return new DemoCleanupInfo(demo, demoDeletionSchedule);
            }
            catch (Exception e)
            {
                return new DemoCleanupInfo(demo, e, DemoState.ErrorAtStateEvaluation);
            }
        }

        private async Task UpdateDeletionScheduleAsync(Demo demo)
        {
            var latestConnection = await _sessionLogsService.GetLatestAsync(demo.Href);
            var latestDemoUsage = latestConnection == new DateTime(0001, 1, 1)
                ? demo.CreatedAt
                : latestConnection;

            var shouldBeDeletedAt = _deletionCalculator.GetDeletionDate(demo, latestDemoUsage);
            await _demosStore.UpdateDeletionScheduleAsync(demo, shouldBeDeletedAt);
        }
    }
}
