using Authentication.Domain;
using Distributors.Domain;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.Instances.Services;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public enum DemoDuplicationRequestSource
    {
        Api = 0,
        Hubspot = 1
    }

    public class DemoDuplicator
    {
        private readonly IDemosStore _demosStore;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IRightsService _rightsService;
        private readonly IDistributorsStore _distributorsStore;
        private readonly ISubdomainValidator _subdomainValidator;
        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IDemoRightsFilter _demoRightsFilter;
        private readonly IDemoUsersPasswordResetService _usersPasswordResetService;

        public DemoDuplicator
        (
            IDemosStore demosStore,
            IDemoDuplicationsStore duplicationsStore,
            IInstancesStore instancesStore,
            IRightsService rightsService,
            IDistributorsStore distributorsStore,
            ISubdomainValidator subdomainValidator,
            IUsersPasswordHelper passwordHelper,
            IDemoRightsFilter demoRightsFilter,
            IDemoUsersPasswordResetService usersPasswordResetService
        )
        {
            _demosStore = demosStore;
            _duplicationsStore = duplicationsStore;
            _instancesStore = instancesStore;
            _rightsService = rightsService;
            _distributorsStore = distributorsStore;
            _subdomainValidator = subdomainValidator;
            _passwordHelper = passwordHelper;
            _demoRightsFilter = demoRightsFilter;
            _usersPasswordResetService = usersPasswordResetService;
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

            var targetSubdomain = await GetSubdomainAsync(request, requestSource);
            var demoToDuplicate = await GetDemoToDuplicateAsync(request.SourceDemoSubdomain, principal);

            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                Type = InstanceDuplicationType.Demos,
                AuthorId = GetAuthorId(principal),
                DistributorId = request.DistributorId,
                SourceCluster = demoToDuplicate.Instance.Cluster,
                TargetCluster = GetTargetCluster(),
                SourceSubdomain = demoToDuplicate.Subdomain,
                TargetSubdomain = targetSubdomain
            };

            var duplication = new DemoDuplication
            {
                InstanceDuplication = instanceDuplication,
                SourceDemoId = demoToDuplicate.Id,
                CreatedAt = DateTime.Now,
                AuthorId = GetAuthorId(principal),
                Password = request.Password,
                Progress = DemoDuplicationProgress.Pending,
                Comment = request.Comment
            };

            await _duplicationsStore.CreateAsync(duplication);
            // TODO call server target


            return duplication;
        }

        private int GetAuthorId(ClaimsPrincipal principal)
        {
            if (!( principal is CloudControlUserClaimsPrincipal user ))
            {
                return 0;
            }

            return user.UserId.Value;
        }

        public async Task MarkDuplicationAsCompleted(Guid instanceDuplicationId)
        {
            var duplication = _duplicationsStore.GetAll()
                .Single(d => d.InstanceDuplicationId == instanceDuplicationId);

            var clusterTarget = GetTargetCluster();

            var instance = await _instancesStore.CreateForDemoAsync(duplication.Password, clusterTarget);
            var demo = CreateDemo(duplication, instance);
            await _demosStore.CreateAsync(demo);
            await _usersPasswordResetService.ResetPasswordAsync(demo, duplication.Password);

            // TODO sync ws auth
            // TODO create SSO for demo if necessary
            // duplication.Status = DuplicationStatus.Success;
        }

        private string GetSourceDemoCluster(string sourceDemoSubdomain)
        {
            var demo = _demosStore
                .GetAll()
                .SingleOrDefault(d => d.IsActive && d.Subdomain == sourceDemoSubdomain);

            return demo?.Instance.Cluster
                   ?? throw new ApplicationException($"Demo {sourceDemoSubdomain} could not be found");
        }

        private string GetTargetCluster()
        {
            return "not-an-actual-demo-cluster";
        }

        private Demo CreateDemo(DemoDuplication duplication, Instance instance)
        {
            return new Demo
            {
                Subdomain = duplication.InstanceDuplication.TargetSubdomain,
                DistributorID = duplication.DistributorId,
                Comment = duplication.Comment,
                CreatedAt = DateTime.Now,
                DeletionScheduledOn = DateTime.Now.AddDays(62),
                IsActive = true,
                IsTemplate = false,
                InstanceID =  instance.Id
            };
        }

        private void ThrowIfInvalid(DemoDuplicationRequest request)
        {
            _passwordHelper.ThrowIfInvalid(request.Password);
        }

        private async Task<string> GetSubdomainAsync(DemoDuplicationRequest request, DemoDuplicationRequestSource source)
        {
            await _subdomainValidator.ThrowIfInvalidAsync(request.Subdomain);

            if (_subdomainValidator.IsAvailable(request.Subdomain))
            {
                return request.Subdomain;
            }

            if (source != DemoDuplicationRequestSource.Hubspot)
            {
                throw new BadRequestException($"Subdomain {request.Subdomain} is not available");
            }

            var availableSubdomain = _subdomainValidator.GetAvailableSubdomain(request.Subdomain);
            if (string.IsNullOrEmpty(availableSubdomain))
            {
                throw new BadRequestException($"Subdomain {request.Subdomain} is not available");
            }

            return availableSubdomain;
        }

        private async Task<Demo> GetDemoToDuplicateAsync(string subdomain, ClaimsPrincipal principal)
        {
            var rightsFilter = await _demoRightsFilter.GetDefaultReadFilterAsync(principal);
            var demoToDuplicate = (await _demosStore.GetAsync(rightsFilter))
                .FirstOrDefault(d => d.Subdomain == subdomain);

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

            if (!( claimsPrincipal is CloudControlUserClaimsPrincipal user))
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
