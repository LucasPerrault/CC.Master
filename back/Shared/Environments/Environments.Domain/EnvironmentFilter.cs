using Authentication.Domain;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Environments.Domain
{
    public interface IEnvironmentFilter
    {
        Expression<Func<Environment, bool>> DepartmentReadAccessFilter(User principal);
        Task<Expression<Func<Environment, bool>>> PurposeReadAccessFilter(Operation operation);
    }

    public class EnvironmentFilter : IEnvironmentFilter
    {
        private readonly IRightsService _rightsService;

        public EnvironmentFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public Expression<Func<Environment, bool>> DepartmentReadAccessFilter(User principal)
        {
            return e => e.ActiveAccesses.Any(a => a.Consumer.Code == principal.DepartmentCode);
        }

        public async Task<Expression<Func<Environment, bool>>> PurposeReadAccessFilter(Operation operation)
        {
            var purposes = await _rightsService.GetEnvironmentPurposesAsync(operation);
            return e => purposes.Contains((int)e.Purpose);
        }
    }
}
