using Environments.Domain;
using Environments.Domain.Storage;
using Storage.Infra.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsStore : IEnvironmentsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Environment> GetFiltered(params Expression<System.Func<Environment, bool>>[] filters)
        {
            return _dbContext.Set<Environment>().Where(filters.CombineSafely());
        }

        public IQueryable<Environment> GetAll()
        {
            return _dbContext.Set<Environment>();
        }
    }
}
