using Authentication.Domain;
using Distributors.Domain;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.DataDuplication;
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
    public class DemoDuplicator
    {
        private readonly IDemosStore _demosStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IRightsService _rightsService;
        private readonly IDistributorsStore _distributorsStore;
        private readonly ISubdomainValidator _subdomainValidator;
        private readonly ITenantDataDuplicator _tenantDataDuplicator;
        private readonly IUsersPasswordHelper _passwordHelper;
        private readonly IDemoRightsFilter _demoRightsFilter;
        private readonly IDemoUsersPasswordResetService _usersPasswordResetService;

        public DemoDuplicator
        (
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            IRightsService rightsService,
            IDistributorsStore distributorsStore,
            ISubdomainValidator subdomainValidator,
            ITenantDataDuplicator tenantDataDuplicator,
            IUsersPasswordHelper passwordHelper,
            IDemoRightsFilter demoRightsFilter,
            IDemoUsersPasswordResetService usersPasswordResetService
        )
        {
            _demosStore = demosStore;
            _instancesStore = instancesStore;
            _rightsService = rightsService;
            _distributorsStore = distributorsStore;
            _subdomainValidator = subdomainValidator;
            _tenantDataDuplicator = tenantDataDuplicator;
            _passwordHelper = passwordHelper;
            _demoRightsFilter = demoRightsFilter;
            _usersPasswordResetService = usersPasswordResetService;
        }

        public async Task CreateDuplicationAsync(DemoDuplication duplication, ClaimsPrincipal principal)
        {
            await ThrowIfForbiddenAsync(duplication, principal);
            ThrowIfInvalid(duplication);
            var subdomain = await GetSubdomainAsync(duplication);
            var demoToDuplicate = await GetDemoToDuplicateAsync(duplication, principal);
            var clusterTarget = GetTargetCluster();

            // TODO storage commit
            // TODO call server target
            // TODO return duplication

        }

        private async Task<DuplicateInstanceRequestDto> GetDuplicationInstanceRequestAsync(Guid duplicationId, ClaimsPrincipal principal)
        {
            DemoDuplication duplication = null; // TODO get duplication by id

            var distributor = await _distributorsStore.GetByIdAsync(duplication.DistributorId);

            var demoToDuplicate = await GetDemoToDuplicateAsync(duplication, principal);

            var databaseDuplication = new TenantDataDuplication
            {
                AuthorId = 0, // TODO  duplication.AuthorId
                Distributor = distributor,
                Type = DatabaseType.Demos,
                Source = new TenantDataSource
                {
                    Subdomain = duplication.Subdomain,
                    ClusterName = GetSourceDemoCluster(duplication.SourceDemoSubdomain)
                },
                Target = new TenantDataSource
                {
                    Subdomain = demoToDuplicate.Subdomain,
                    ClusterName = demoToDuplicate.Instance.Cluster
                }
            };

            return await _tenantDataDuplicator.DuplicateOnRemoteAsync(databaseDuplication);
        }

        private async Task MarkDuplicationAsCompleted()
        {
            DemoDuplication duplication = null; // TODO  get duplication by id
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
                .GetAllAsync()
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
                Subdomain = duplication.Subdomain,
                DistributorID = duplication.DistributorId,
                Comment = duplication.Comment,
                CreatedAt = DateTime.Now,
                DeletionScheduledOn = DateTime.Now.AddDays(62),
                IsActive = true,
                IsTemplate = false,
                InstanceID =  instance.Id
            };
        }

        private void ThrowIfInvalid(DemoDuplication duplication)
        {
            _passwordHelper.ThrowIfInvalid(duplication.Password);
        }

        private async Task<string> GetSubdomainAsync(DemoDuplication duplication)
        {
            await _subdomainValidator.ThrowIfInvalidAsync(duplication.Subdomain);

            if (_subdomainValidator.IsAvailable(duplication.Subdomain))
            {
                return duplication.Subdomain;
            }

            if (duplication.IsStrictSubdomainSelection)
            {
                throw new BadRequestException($"Subdomain {duplication.Subdomain} is not available");
            }

            var availableSubdomain = _subdomainValidator.GetAvailableSubdomain(duplication.Subdomain);
            if (string.IsNullOrEmpty(availableSubdomain))
            {
                throw new BadRequestException($"Subdomain {duplication.Subdomain} is not available");
            }

            return availableSubdomain;
        }

        private async Task<Demo> GetDemoToDuplicateAsync(DemoDuplication duplication, ClaimsPrincipal principal)
        {
            var rightsFilter = await _demoRightsFilter.GetDefaultReadFilterAsync(principal);
            var demoToDuplicate = (await _demosStore.GetAsync(rightsFilter))
                .FirstOrDefault(d => d.Subdomain == duplication.SourceDemoSubdomain);

            return demoToDuplicate
                ?? throw new BadRequestException($"Source demo {duplication.SourceDemoSubdomain} could not be found");
        }

        private async Task ThrowIfForbiddenAsync(DemoDuplication duplication, ClaimsPrincipal claimsPrincipal)
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
            if (userDistributor.Id == duplication.DistributorId)
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
