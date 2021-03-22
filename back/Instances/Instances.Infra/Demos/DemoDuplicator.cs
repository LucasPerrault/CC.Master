using Authentication.Domain;
using Distributors.Infra.Storage.Stores;
using Instances.Domain.Demos;
using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Infra.Demos
{
    public class DemoDuplicator
    {
        private readonly IRightsService _rightsService;
        private readonly DistributorsStore _distributorsStore;

        public DemoDuplicator(IRightsService rightsService, DistributorsStore distributorsStore)
        {
            _rightsService = rightsService;
            _distributorsStore = distributorsStore;
        }

        public async Task DuplicateAsync(DemoDuplication duplication, ClaimsPrincipal principal)
        {
            await ThrowIfForbiddenAsync(duplication, principal);

            // ensure subdomain is available, source demo exists, distributor selection is okay...

            // determine sql scripts to run on newly copied db

            // Request remote database creation

            // create demo on local

            // change password for all users

            // create SSO for demo if necessary
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
