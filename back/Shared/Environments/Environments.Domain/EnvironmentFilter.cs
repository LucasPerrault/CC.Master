using Authentication.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Environments.Domain
{
    public interface IEnvironmentFilter
    {
        Expression<Func<Environment, bool>> ReadAccessFilter(User principal);
    }

    public class EnvironmentFilter : IEnvironmentFilter
    {
        public Expression<Func<Environment, bool>> ReadAccessFilter(User principal)
        {
            return e => e.ActiveAccesses.Any(a => a.Consumer.Code == principal.DepartmentCode);
        }
    }
}
