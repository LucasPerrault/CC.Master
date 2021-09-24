using Authentication.Domain;
using Authentication.Domain.Helpers;
using Distributors.Domain;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos.Duplication
{
    public class DemoDuplicator
    {
        private readonly ClaimsPrincipal _principal;
        private readonly InstancesManipulator _instancesDuplicator;
        private readonly IDemosStore _demosStore;
        private readonly IDemoDuplicationsStore _duplicationsStore;
        private readonly IRightsService _rightsService;
        private readonly IDistributorsStore _distributorsStore;
        private readonly ISubdomainGenerator _subdomainGenerator;
        private readonly IClusterSelector _clusterSelector;
        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IDnsService _dnsService;
        private readonly DemoRightsFilter _demoRightsFilter;

        public DemoDuplicator
        (
            ClaimsPrincipal principal,
            InstancesManipulator instancesDuplicator,
            IDemosStore demosStore,
            IDemoDuplicationsStore duplicationsStore,
            IRightsService rightsService,
            IDistributorsStore distributorsStore,
            ISubdomainGenerator subdomainGenerator,
            IClusterSelector clusterSelector,
            IUsersPasswordHelper passwordHelper,
            IDnsService dnsService
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
            _dnsService = dnsService;
            _demoRightsFilter = new DemoRightsFilter(new RightsFilter(_rightsService));
        }

        public async Task<DemoDuplication> CreateDuplicationAsync(DemoDuplicationRequest request)
        {
            await ThrowIfForbiddenAsync(request);
            ThrowIfInvalid(request);
            await _subdomainGenerator.ThrowIfNotUsable(request.Subdomain);

            var demoToDuplicate = await GetDemoToDuplicateAsync(request.SourceId);
            var distributor = await _distributorsStore.GetActiveByCodeAsync(request.DistributorCode);
            var targetCluster = await _clusterSelector.GetFillingClusterAsync(request.Subdomain);

            var duplication = DemoDuplicationFactory.New
            (
                distributor.Id,
                GetAuthorId(_principal),
                demoToDuplicate,
                targetCluster,
                request.Subdomain,
                request.Password,
                request.Comment
            );

            await _dnsService.CreateAsync(DnsEntry.ForDemo(request.Subdomain, targetCluster));
            await _duplicationsStore.CreateAsync(duplication);
            await _instancesDuplicator.RequestRemoteDuplicationAsync
            (
                duplication.InstanceDuplication,
                $"/api/demos/duplications/{duplication.InstanceDuplicationId}/notify"
            );

            return duplication;
        }

        private int GetAuthorId(ClaimsPrincipal principal)
        {
            var authorId = _principal.GetAuthorIdOnlyWhenUser();
            if (authorId == null)
            {
                throw new BadRequestException("Duplication api is opened to users only");
            }

            return authorId.Value;
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

            var distributor = await _distributorsStore.GetActiveByCodeAsync(request.DistributorCode);
            if (user.User.DistributorId == distributor.Id)
            {
                return;
            }

            var scope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.Demo);
            if (scope != AccessRightScope.AllDistributors)
            {
                throw new ForbiddenException("Insufficient rights to duplicate demo for another department than your own");
            }
        }
    }
}
