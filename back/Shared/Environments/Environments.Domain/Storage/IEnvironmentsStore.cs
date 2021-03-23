using System;
using System.Linq;
using System.Linq.Expressions;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        IQueryable<Environment> GetFilteredAsync(params Expression<Func<Environment, bool>>[] filters);
        IQueryable<Environment> GetAllAsync();
    }
}
