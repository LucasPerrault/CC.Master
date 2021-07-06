using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Distributors.Infra.Storage.Stores
{
    public class DistributorDomainsStore
    {
        private readonly DistributorsDbContext _dbContext;

        public DistributorDomainsStore(DistributorsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<DistributorDomain> GetByDistributorId(int distributorId)
        {
            return _dbContext.Set<DistributorDomain>()
                .Where(d => d.DistributorId == distributorId);
        }
    }
}
