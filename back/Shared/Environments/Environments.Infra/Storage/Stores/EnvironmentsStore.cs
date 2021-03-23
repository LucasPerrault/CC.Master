using Environments.Domain.Storage;
using Storage.Infra.Stores;
using System;
using System.Linq;
using System.Linq.Expressions;
using Environment = Environments.Domain.Environment;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsStore : IEnvironmentsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Environment> GetFilteredAsync(params Expression<Func<Environment, bool>>[] filters)
        {
            return _dbContext.Set<Environment>().Where(filters.CombineSafely());
        }

        public IQueryable<Environment> GetAllAsync()
        {
            return _dbContext.Set<Environment>();
        }
    }
}
