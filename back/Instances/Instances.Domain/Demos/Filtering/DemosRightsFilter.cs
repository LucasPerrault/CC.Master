using Authentication.Domain;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Domain.Demos.Filtering
{
    public class DemoRightsFilter
    {
        private readonly IRightsService _rightsService;

        public DemoRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public async Task<AccessRight> GetReadAccessAsync(ClaimsPrincipal principal)
        {
            switch(principal)
            {
                case CloudControlUserClaimsPrincipal userPrincipal:
                    var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.Demo);
                    return currentUserScope switch
                    {
                        Scope.AllDepartments => AccessRight.All,
                        Scope.DepartmentOnly => AccessRight.ForDistributor(userPrincipal.User.DepartmentCode),
                        _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
                    };
                case CloudControlApiKeyClaimsPrincipal _:
                    return AccessRight.All;
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            };
        }
    }
}
