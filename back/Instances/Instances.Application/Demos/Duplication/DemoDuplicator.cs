using Authentication.Domain;
using Distributors.Domain;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.Instances.Services;
using Instances.Infra.WsAuth;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos.Duplication
{
    public enum DemoDuplicationRequestSource
    {
        Api = 0,
        Hubspot = 1
    }

    public class DemoDuplicator
    {
        private readonly InstancesDuplicator _instancesDuplicator;
        private readonly IDemosStore _demosStore;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly IInstanceDuplicationsStore _instanceDuplicationsStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IRightsService _rightsService;
        private readonly IDistributorsStore _distributorsStore;
        private readonly ISubdomainGenerator _subdomainGenerator;
        private readonly IClusterSelector _clusterSelector;
        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IDemoRightsFilter _demoRightsFilter;
        private readonly IDemoUsersPasswordResetService _usersPasswordResetService;
        private readonly IWsAuthSynchronizer _wsAuthSynchronizer;
        private readonly IDemoDeletionCalculator _deletionCalculator;
        private readonly ILogger<DemoDuplicator> _logger;

        public DemoDuplicator
        (
            InstancesDuplicator instancesDuplicator,
            IDemosStore demosStore,
            IDemoDuplicationsStore duplicationsStore,
            IInstanceDuplicationsStore instanceDuplicationsStore,
            IInstancesStore instancesStore,
            IRightsService rightsService,
            IDistributorsStore distributorsStore,
            ISubdomainGenerator subdomainGenerator,
            IClusterSelector clusterSelector,
            IUsersPasswordHelper passwordHelper,
            IDemoRightsFilter demoRightsFilter,
            IDemoUsersPasswordResetService usersPasswordResetService,
            IWsAuthSynchronizer wsAuthSynchronizer,
            IDemoDeletionCalculator deletionCalculator,
            ILogger<DemoDuplicator> logger
        )
        {
            _instancesDuplicator = instancesDuplicator;
            _demosStore = demosStore;
            _duplicationsStore = duplicationsStore;
            _instanceDuplicationsStore = instanceDuplicationsStore;
            _instancesStore = instancesStore;
            _rightsService = rightsService;
            _distributorsStore = distributorsStore;
            _subdomainGenerator = subdomainGenerator;
            _clusterSelector = clusterSelector;
            _passwordHelper = passwordHelper;
            _demoRightsFilter = demoRightsFilter;
            _usersPasswordResetService = usersPasswordResetService;
            _wsAuthSynchronizer = wsAuthSynchronizer;
            _deletionCalculator = deletionCalculator;
            _logger = logger;
        }

        public async Task<DemoDuplication> CreateDuplicationAsync
        (
            DemoDuplicationRequest request,
            DemoDuplicationRequestSource requestSource,
            ClaimsPrincipal principal
        )
        {
            await ThrowIfForbiddenAsync(request, principal);
            ThrowIfInvalid(request);

            var shouldUseSubdomainAsPrefix = requestSource == DemoDuplicationRequestSource.Hubspot;
            var targetSubdomain = await _subdomainGenerator.GetSubdomainAsync(request.Subdomain, shouldUseSubdomainAsPrefix);

            var demoToDuplicate = await GetDemoToDuplicateAsync(request.SourceDemoSubdomain, principal);

            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                Type = InstanceDuplicationType.Demos,
                DistributorId = request.DistributorId,
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
                AuthorId = GetAuthorId(principal),
                Password = request.Password,
                Comment = request.Comment
            };

            await _duplicationsStore.CreateAsync(duplication);
            await _instancesDuplicator.RequestRemoteDuplicationAsync(instanceDuplication, requestSource);

            return duplication;
        }

        private int GetAuthorId(ClaimsPrincipal principal)
        {
            if (!(principal is CloudControlUserClaimsPrincipal user))
            {
                return 0;
            }

            return user.UserId.Value;
        }

        public async Task MarkDuplicationAsCompletedAsync(Guid instanceDuplicationId, bool isSuccessful)
        {
            var duplication = _duplicationsStore.GetByInstanceDuplicationId(instanceDuplicationId);

            isSuccessful = isSuccessful && await CompleteDemoCreationAsync(duplication);

            var success = isSuccessful ? InstanceDuplicationProgress.FinishedWithSuccess : InstanceDuplicationProgress.FinishedWithFailure;
            await _instanceDuplicationsStore.UpdateProgressAsync(duplication.InstanceDuplication, success);
        }

        private async Task<bool> CompleteDemoCreationAsync(DemoDuplication duplication)
        {
            Instance instance;
            try
            {
                instance = await _instancesStore.CreateForDemoAsync(duplication.Password, duplication.InstanceDuplication.TargetCluster);
                var demo = BuildDemo(duplication, instance);
                await _demosStore.CreateAsync(demo);
                await _usersPasswordResetService.ResetPasswordAsync(demo, duplication.Password);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create demo, following instance duplication");
                return false;
            }

            await _wsAuthSynchronizer.SafeSynchronizeAsync(instance.Id);

            return true;
        }

        private Demo BuildDemo(DemoDuplication duplication, Instance instance)
        {
            var demo = new Demo
            {
                Subdomain = duplication.InstanceDuplication.TargetSubdomain,
                DistributorID = duplication.DistributorId,
                Comment = duplication.Comment,
                CreatedAt = DateTime.Now,
                AuthorId = duplication.AuthorId,
                IsActive = true,
                IsTemplate = false,
                InstanceID = instance.Id
            };

            demo.DeletionScheduledOn = _deletionCalculator.GetDeletionDate(demo, DateTime.Now);
            return demo;
        }

        private void ThrowIfInvalid(DemoDuplicationRequest request)
        {
            _passwordHelper.ThrowIfInvalid(request.Password);
        }

        private async Task<Demo> GetDemoToDuplicateAsync(string subdomain, ClaimsPrincipal principal)
        {
            var rightsFilter = await _demoRightsFilter.GetDefaultReadFilterAsync(principal);
            var demoToDuplicate = (await _demosStore.GetActiveAsync(rightsFilter, d => d.Subdomain == subdomain))
                .FirstOrDefault();

            return demoToDuplicate
                ?? throw new BadRequestException($"Source demo {subdomain} could not be found");
        }

        private async Task ThrowIfForbiddenAsync(DemoDuplicationRequest request, ClaimsPrincipal claimsPrincipal)
        {
            await _rightsService.ThrowIfAnyOperationIsMissingAsync(Operation.Demo);

            if (claimsPrincipal is CloudControlApiKeyClaimsPrincipal)
            {
                return;
            }

            if (!(claimsPrincipal is CloudControlUserClaimsPrincipal user))
            {
                throw new ApplicationException("Unsupported claims principal type");
            }

            var userDistributor = await _distributorsStore.GetByCodeAsync(user.User.DepartmentCode);
            if (userDistributor.Id == request.DistributorId)
            {
                return;
            }

            var scope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.Demo);
            if (scope != Scope.AllDepartments)
            {
                throw new ForbiddenException("Insufficient rights to duplicate demo for another department than your own");
            }
        }
    }
}
