using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Infra.Storage.Stores
{
    public class SolutionsStore : ISolutionsStore
    {
        private readonly ProductDbContext _dbContext;

        public SolutionsStore(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Solution>> GetAsync()
        {
            return _dbContext.Set<Solution>()
                .Include(s => s.BusinessUnit)
                .ToListAsync();
        }
    }
}
