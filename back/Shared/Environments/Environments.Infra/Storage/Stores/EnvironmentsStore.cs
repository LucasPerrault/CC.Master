using Environments.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Environment> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idsSet = new HashSet<int>(ids);
            return _dbContext.Set<Environment>().Where(e => idsSet.Contains(e.Id));
        }
    }
}
