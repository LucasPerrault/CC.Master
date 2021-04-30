using Authentication.Domain;
using Environments.Domain;
using Environments.Domain.Storage;
using Rights.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Environments.Application
{
    public class EnvironmentsRepository
    {
        private readonly IEnvironmentsStore _store;
        private readonly ClaimsPrincipal _principal;
        private readonly IEnvironmentFilter _filter;

        public EnvironmentsRepository(IEnvironmentsStore store, ClaimsPrincipal principal, IEnvironmentFilter filter)
        {
            _store = store;
            _principal = principal;
            _filter = filter;
        }

        public async Task<List<Environment>> GetAsync()
        {
            var filters = new List<Expression<System.Func<Environment, bool>>>
            {
                await _filter.PurposeReadAccessFilter(Operation.ReadEnvironments)
            };

            if (_principal is CloudControlUserClaimsPrincipal user)
            {
                filters.Add(_filter.DepartmentReadAccessFilter(user.User));
            }

            return _store
                .GetFiltered(filters.ToArray())
                .ToList();
        }
    }
}
