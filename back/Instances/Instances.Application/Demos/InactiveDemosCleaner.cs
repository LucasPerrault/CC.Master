using Email.Domain;
using Instances.Application.Demos.Emails;
using Instances.Domain.Demos;
using Instances.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public class InactiveDemosCleaner
    {
        private readonly HttpClient _httpClient;
        private readonly IDemosStore _demosStore;
        private readonly ICcDataService _ccDataService;
        private readonly IEmailService _emailService;

        public static readonly TimeSpan StandardAcceptableInactivity = TimeSpan.FromDays(62);
        public static readonly TimeSpan HubspotAcceptableInactivity = TimeSpan.FromDays(31);

        private const int HubspotUserId = 0;

        public InactiveDemosCleaner
        (
            HttpClient httpClient,
            IDemosStore demosStore,
            ICcDataService ccDataService,
            IEmailService emailService
        )
        {
            _httpClient = httpClient;
            _demosStore = demosStore;
            _ccDataService = ccDataService;
            _emailService = emailService;
        }

        public async Task CleanAsync()
        {
            var infoTasks = ( await _demosStore.GetActiveAsync() )
                .Where(d => !d.IsTemplate && !d.Instance.IsProtected)
                .Select(d => GetUpdatedCleanupInfoAsync(d));

            var infos = await Task.WhenAll(infoTasks);

            await ReportCleanupIntentionsAsync(infos.Where(i => i.ShouldBeReported()));
            await DeleteAllAsync(infos.Where(i => i.State == DemoState.DeletionScheduledToday));
        }

        private TimeSpan GetAcceptableInactivity(Demo demo)
        {
            if (demo.AuthorId == HubspotUserId)
            {
                return HubspotAcceptableInactivity;
            }

            return StandardAcceptableInactivity;
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
                DemoEmails.IntentEmail(DateTime.Today, info).Content
            );
        }

        private async Task<DateTime> GetLatestConnectionAsync(Demo demo)
        {
            var uri = new Uri(demo.Href, "/api/v3/sessionlogs/latest");
            var dateTimeAsString = await _httpClient.GetStringAsync(uri);
            return DateTime.Parse(dateTimeAsString);
        }

        private async Task<DemoCleanupInfo> GetUpdatedCleanupInfoAsync(Demo demo)
        {
            try
            {
                await UpdateDeletionScheduleAsync(demo);
                return new DemoCleanupInfo(demo);
            }
            catch (Exception e)
            {
                return new DemoCleanupInfo(demo, e, DemoState.ErrorAtStateEvaluation);
            }
        }

        private async Task UpdateDeletionScheduleAsync(Demo demo)
        {
            var latestConnection = await GetLatestConnectionAsync(demo);
            var acceptableInactivity = GetAcceptableInactivity(demo);
            await _demosStore.UpdateDeletionScheduleAsync(demo, latestConnection.Add(acceptableInactivity));
        }
    }
}
