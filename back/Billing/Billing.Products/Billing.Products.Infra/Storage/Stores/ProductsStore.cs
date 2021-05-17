using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Infra.Storage.Stores
{
    public class ProductsStore : IProductsStore
    {
        private readonly ProductDbContext _dbContext;

        public ProductsStore(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return _dbContext.Set<Product>()
                .Include(p => p.Family)
                .ToListAsync();
        }
    }
}
