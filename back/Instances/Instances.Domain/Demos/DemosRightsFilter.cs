using Authentication.Domain;
using Lucca.Core.Rights.Abstractions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IDemoRightsFilter
    {
        Task<Expression<Func<Demo, bool>>> GetDefaultReadFilterAsync(ClaimsPrincipal principal);
    }

    public class DemoRightsFilter : IDemoRightsFilter
    {
        private readonly IRightsService _rightsService;

        public DemoRightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public async Task<Expression<Func<Demo, bool>>> GetDefaultReadFilterAsync(ClaimsPrincipal principal)
        {
            switch(principal)
            {
                case CloudControlUserClaimsPrincipal userPrincipal:
                    var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.Demo);
                    return currentUserScope switch
                    {
                        Scope.AllDepartments => _ => true,
                        Scope.DepartmentOnly => d => d.Distributor.Code == userPrincipal.User.DepartmentCode || d.IsTemplate,
                        _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
                    };
                case CloudControlApiKeyClaimsPrincipal apiKey:
                    return (Demo d) => true;
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            };
        }

        public async Task<Expression<Func<DemoDuplication, bool>>> GetDuplicationReadFilterAsync(ClaimsPrincipal principal)
        {
            switch(principal)
            {
                case CloudControlUserClaimsPrincipal userPrincipal:
                    var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(Operation.Demo);
                    return currentUserScope switch
                    {
                        Scope.AllDepartments => _ => true,
                        Scope.DepartmentOnly => d => d.InstanceDuplication.Distributor.Code == userPrincipal.User.DepartmentCode,
                        _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
                    };
                case CloudControlApiKeyClaimsPrincipal apiKey:
                    return _ => true;
                default:
                    throw new ApplicationException("Unhandled ClaimsPrincipal type");
            };
        }
    }
}
