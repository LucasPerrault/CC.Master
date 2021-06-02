using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<Product>> GetNonFreeProductsAsync()
        {
            return _dbContext.Set<Product>()
                .Include(p => p.Family)
                .Include(p => p.ProductSolutions).ThenInclude(ps => ps.Solution)
                .Where(p => !p.IsFreeUse)
                .ToListAsync();
        }
    }
}
