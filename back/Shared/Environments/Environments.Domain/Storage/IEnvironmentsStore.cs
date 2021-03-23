using System;
using System.Linq;
using System.Linq.Expressions;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        IQueryable<Environment> GetFilteredAsync(Expression<Func<Environment, bool>> filter);
        IQueryable<Environment> GetAllAsync();
    }
}
