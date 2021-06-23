using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
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

        public Task<List<Product>> GetAsync(ProductsFilter filter, ProductsIncludes includes)
        {
            return _dbContext.Set<Product>()
                .WithIncludes(includes)
                .WhereMatches(filter)
                .ToListAsync();
        }
    }

    internal static class ProductsFilteringExtensions
    {
        internal static IQueryable<Product> WhereMatches(this IQueryable<Product> products, ProductsFilter filter)
        {
            return products.When(filter.NonFreeOnly).ApplyWhere(p => !p.IsFreeUse);
        }

        internal static IQueryable<Product> WithIncludes(this IQueryable<Product> products, ProductsIncludes includes)
        {
            if (includes.Families)
            {
                products = products.Include(p => p.Family);
            }

            if (includes.Solutions)
            {
                products = products.Include(p => p.ProductSolutions).ThenInclude(ps => ps.Solution);
            }

            if (includes.BusinessUnits)
            {
                products = products
                    .Include(p => p.ProductSolutions)
                    .ThenInclude(ps => ps.Solution)
                    .ThenInclude(s => s.BusinessUnit);
            }

            return products;
        }
    }
}
