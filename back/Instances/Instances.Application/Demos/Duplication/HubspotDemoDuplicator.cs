using Cache.Abstractions;
using Distributors.Domain.Models;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Demos.Duplication
{
    public class HubspotDemoDuplication
    {
        public int VId { get; set; }
        public int FailureWorkflowId { get; set; }
        public int SuccessWorkflowId { get; set; }
    }

    public class HubspotCachedDuplication
    {
        public string Email { get; set; }
        public int FailureWorkflowId { get; set; }
        public int SuccessWorkflowId { get; set; }
    }

    public class HubspotDemoDuplicationKey : CacheKey<HubspotCachedDuplication>
    {
        public static readonly TimeSpan LifeSpan = TimeSpan.FromMinutes(20);

        private readonly Guid _instanceDuplicationId;
        public override string Key => $"hubspot:duplication:{_instanceDuplicationId.ToString()}";

        public HubspotDemoDuplicationKey(Guid instanceDuplicationId)
        {
            _instanceDuplicationId = instanceDuplicationId;
        }
    }

    public class HubspotDemoDuplicator
    {
        private const int DefaultSourceDemoId = 385;
        private const string DefaultHubspotPassword = "test";

        private readonly DemoDuplicator _demoDuplicator;
        private readonly ICacheService _cacheService;
        private readonly IHubspotService _hubspotService;
        private readonly IDemosStore _demosStore;
        private readonly ISubdomainGenerator _subdomainGenerator;
        private readonly IClusterSelector _clusterSelector;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly InstancesDuplicator _instancesDuplicator;

        public HubspotDemoDuplicator
        (
            DemoDuplicator demoDuplicator,
            ICacheService cacheService,
            IHubspotService hubspotService,
            IDemosStore demosStore,
            ISubdomainGenerator subdomainGenerator,
            IClusterSelector clusterSelector,
            IDemoDuplicationsStore duplicationsStore,
            InstancesDuplicator instancesDuplicator
        )
        {
            _demoDuplicator = demoDuplicator;
            _cacheService = cacheService;
            _hubspotService = hubspotService;
            _demosStore = demosStore;
            _subdomainGenerator = subdomainGenerator;
            _clusterSelector = clusterSelector;
            _duplicationsStore = duplicationsStore;
            _instancesDuplicator = instancesDuplicator;
        }

        public async Task DuplicateMasterForHubspotAsync(HubspotDemoDuplication hubspotDemoDuplication)
        {
            var contact = await _hubspotService.GetContactAsync(hubspotDemoDuplication.VId);

            var demoDuplication = await CreateDuplicationAsync(contact.Company.ToValidSubdomain());
            await _hubspotService.UpdateContactSubdomainAsync
            (
                contact.VId,
                demoDuplication.InstanceDuplication.TargetSubdomain
            );

            var cachedDuplication = new HubspotCachedDuplication
            {
                Email = contact.Email,
                FailureWorkflowId = hubspotDemoDuplication.FailureWorkflowId,
                SuccessWorkflowId = hubspotDemoDuplication.SuccessWorkflowId
            };
            var cacheKey = new HubspotDemoDuplicationKey(demoDuplication.InstanceDuplicationId);
            await _cacheService.SetAsync
            (
                cacheKey,
                cachedDuplication,
                CacheInvalidation.After(HubspotDemoDuplicationKey.LifeSpan)
            );
        }

        private async Task<DemoDuplication> CreateDuplicationAsync(string subdomain)
        {
            var targetSubdomain = await _subdomainGenerator.GetSubdomainAsync(subdomain, true);
            var demoToDuplicate = await _demosStore.GetActiveByIdAsync(DefaultSourceDemoId, DemoAccess.All);

            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                SourceType = InstanceType.Demo,
                TargetType = InstanceType.Demo,
                DistributorId = DistributorIds.Lucca,
                SourceCluster = demoToDuplicate.Instance.Cluster,
                TargetCluster = await _clusterSelector.GetFillingCluster(),
                SourceSubdomain = demoToDuplicate.Subdomain,
                TargetSubdomain = targetSubdomain,
                Progress = InstanceDuplicationProgress.Pending
            };

            var duplication = new DemoDuplication
            {
                InstanceDuplication = instanceDuplication,
                SourceDemoId = demoToDuplicate.Id,
                CreatedAt = DateTime.Now,
                AuthorId = 0,
                Password = DefaultHubspotPassword,
            };

            await _duplicationsStore.CreateAsync(duplication);
            await _instancesDuplicator.RequestRemoteDuplicationAsync(instanceDuplication, $"/api/hubspot/duplications/{duplication.Id}/notify");

            return duplication;
        }

        public async Task MarkAsEndedAsync(Guid instanceDuplicationId, bool isSuccessful)
        {
            var key = new HubspotDemoDuplicationKey(instanceDuplicationId);

            await _demoDuplicator.MarkDuplicationAsCompletedAsync(instanceDuplicationId, isSuccessful);

            var cachedDuplication = await _cacheService.GetAsync(key);

            var workflowId = isSuccessful
                ? cachedDuplication.SuccessWorkflowId
                : cachedDuplication.FailureWorkflowId;

            await _hubspotService.CallWorkflowForEmailAsync(workflowId, cachedDuplication.Email);
            await _cacheService.ExpireAsync(key);
        }
    }
}
