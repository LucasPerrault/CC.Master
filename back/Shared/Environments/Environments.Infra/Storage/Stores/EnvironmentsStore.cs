using Environments.Domain;
using System.Linq;
using System.Linq.Expressions;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Environment> GetFilteredAsync(Expression<System.Func<Environment, bool>> filter)
        {
            return _dbContext.Set<Environment>().Where(filter);
        }

        public IQueryable<Environment> GetAllAsync()
        {
            return _dbContext.Set<Environment>();
        }
    }
}
