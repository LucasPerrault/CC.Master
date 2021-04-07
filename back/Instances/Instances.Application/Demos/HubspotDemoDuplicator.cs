using Cache.Abstractions;
using Instances.Domain.Demos;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos
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
        private readonly Guid _instanceDuplicationId;
        public override string Key => $"hubspot:duplication:{_instanceDuplicationId.ToString()}";
        public override TimeSpan? Invalidation => TimeSpan.FromMinutes(20);

        public HubspotDemoDuplicationKey(Guid instanceDuplicationId)
        {
            _instanceDuplicationId = instanceDuplicationId;
        }
    }

    public class HubspotDemoDuplicator
    {
        private const string _defaultMasterSubdomain = "masterdemo";
        private const string _defaultHubspotPassword = "test";

        private readonly DemoDuplicator _demoDuplicator;
        private readonly ClaimsPrincipal _principal;
        private readonly ICacheService _cacheService;
        private readonly IHubspotService _hubspotService;

        public HubspotDemoDuplicator
        (
            DemoDuplicator demoDuplicator,
            ClaimsPrincipal principal,
            ICacheService cacheService,
            IHubspotService hubspotService
        )
        {
            _demoDuplicator = demoDuplicator;
            _principal = principal;
            _cacheService = cacheService;
            _hubspotService = hubspotService;
        }

        public async Task DuplicateAsync(HubspotDemoDuplication hubspotDemoDuplication)
        {
            var contact = await _hubspotService.GetContactAsync(hubspotDemoDuplication.VId);
            var request = new DemoDuplicationRequest
            {
                SourceDemoSubdomain = _defaultMasterSubdomain,

                Subdomain = contact.Company,
                Password = _defaultHubspotPassword
            };

            var demoDuplication = await _demoDuplicator.CreateDuplicationAsync
            (
                request,
                DemoDuplicationRequestSource.Hubspot,
                _principal
            );

            var cachedDuplication = new HubspotCachedDuplication
            {
                Email = contact.Email,
                FailureWorkflowId = hubspotDemoDuplication.FailureWorkflowId,
                SuccessWorkflowId = hubspotDemoDuplication.SuccessWorkflowId
            };
            var cacheKey = new HubspotDemoDuplicationKey(demoDuplication.InstanceDuplicationId);
            await _cacheService.SetAsync(cacheKey, cachedDuplication);
        }
    }
}
