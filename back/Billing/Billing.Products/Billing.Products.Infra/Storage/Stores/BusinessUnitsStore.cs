using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Infra.Storage.Stores
{
    public class BusinessUnitsStore : IBusinessUnitsStore
    {
        private readonly ProductDbContext _dbContext;

        public BusinessUnitsStore(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<BusinessUnit>> GetAsync()
        {
            return _dbContext.Set<BusinessUnit>().ToListAsync();
        }
    }
}
