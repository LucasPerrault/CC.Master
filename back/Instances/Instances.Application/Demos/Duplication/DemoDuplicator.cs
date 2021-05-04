using Authentication.Domain;
using Distributors.Domain;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.Instances.Services;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos.Duplication
{
    public class DemoDuplicator
    {
        private readonly ClaimsPrincipal _principal;
        private readonly InstancesDuplicator _instancesDuplicator;
        private readonly IDemosStore _demosStore;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly IRightsService _rightsService;
        private readonly IDistributorsStore _distributorsStore;
        private readonly ISubdomainGenerator _subdomainGenerator;
        private readonly IClusterSelector _clusterSelector;
        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly DemoRightsFilter _demoRightsFilter;

        public DemoDuplicator
        (
            ClaimsPrincipal principal,
            InstancesDuplicator instancesDuplicator,
            IDemosStore demosStore,
            IDemoDuplicationsStore duplicationsStore,
            IRightsService rightsService,
            IDistributorsStore distributorsStore,
            ISubdomainGenerator subdomainGenerator,
            IClusterSelector clusterSelector,
            IUsersPasswordHelper passwordHelper
        )
        {
            _principal = principal;
            _instancesDuplicator = instancesDuplicator;
            _demosStore = demosStore;
            _duplicationsStore = duplicationsStore;
            _rightsService = rightsService;
            _distributorsStore = distributorsStore;
            _subdomainGenerator = subdomainGenerator;
            _clusterSelector = clusterSelector;
            _passwordHelper = passwordHelper;
            _demoRightsFilter = new DemoRightsFilter(_rightsService);
        }

        public async Task<DemoDuplication> CreateDuplicationAsync(DemoDuplicationRequest request)
        {
            await ThrowIfForbiddenAsync(request);
            ThrowIfInvalid(request);

            var targetSubdomain = await _subdomainGenerator.GetSubdomainAsync(request.Subdomain, false);

            var demoToDuplicate = await GetDemoToDuplicateAsync(request.SourceId);
            var distributor = await _distributorsStore.GetByCodeAsync(request.DistributorCode);
            var instanceDuplication = new InstanceDuplication
            {
                Id = Guid.NewGuid(),
                SourceType = InstanceType.Demo,
                TargetType = InstanceType.Demo,
                DistributorId = distributor.Id,
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
                AuthorId = GetAuthorId(_principal),
                Password = request.Password,
                Comment = request.Comment
            };

            await _duplicationsStore.CreateAsync(duplication);
            await _instancesDuplicator.RequestRemoteDuplicationAsync(instanceDuplication, $"/api/demos/duplications/{duplication.InstanceDuplicationId}/notify");

            return duplication;
        }

        private int GetAuthorId(ClaimsPrincipal principal)
        {
            if (!(principal is CloudControlUserClaimsPrincipal user))
            {
                throw new BadRequestException("Duplication api is opened to users only");
            }

            return user.UserId.Value;
        }

        private void ThrowIfInvalid(DemoDuplicationRequest request)
        {
            _passwordHelper.ThrowIfInvalid(request.Password);
        }

        private async Task<Demo> GetDemoToDuplicateAsync(int sourceId)
        {
            var access = await _demoRightsFilter.GetReadAccessAsync(_principal);
            var demoToDuplicate = await _demosStore.GetActiveByIdAsync(sourceId, access);

            return demoToDuplicate
                ?? throw new BadRequestException($"Source demo {sourceId} could not be found");
        }

        private async Task ThrowIfForbiddenAsync(DemoDuplicationRequest request)
        {
            await _rightsService.ThrowIfAnyOperationIsMissingAsync(Operation.Demo);

            if (_principal is CloudControlApiKeyClaimsPrincipal)
            {
                throw new BadRequestException("Duplication api is opened to users only");
            }

            if (!(_principal is CloudControlUserClaimsPrincipal user))
            {
                throw new ApplicationException("Unsupported claims principal type");
            }

            if (user.User.DepartmentCode == request.DistributorCode)
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
