using Billing.Products.Domain;
using Billing.Products.Infra.Storage;
using Billing.Products.Infra.Storage.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Products.Infra.Tests
{
    public class ProductsStoreTests
    {
        private ProductDbContext _dbContext;

        public ProductsStoreTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _dbContext = new ProductDbContext(options);
        }
        [Fact]
        public async Task Test1()
        {
            var product1 = new Product { Id = 1, Name = "Name1" };
            var product2 = new Product { Id = 2, Name = "Name2" };
            var product3 = new Product { Id = 3, Name = "Name3" };

            var solution1 = new Solution { Id = 10, Name = "Solution1" };
            var solution2 = new Solution { Id = 11, Name = "Solution2" };

            var productSolution1 = new ProductSolution { ProductId = 1, SolutionId = 10 };
            var productSolution2 = new ProductSolution { ProductId = 1, SolutionId = 11 };
            var productSolution3 = new ProductSolution { ProductId = 2, SolutionId = 10 };

            await _dbContext.AddRangeAsync(new List<Product> { product1, product2, product3 });
            await _dbContext.AddRangeAsync(new List<Solution> { solution1, solution2});
            await _dbContext.AddRangeAsync(new List<ProductSolution> { productSolution1, productSolution2, productSolution3 });

            await _dbContext.SaveChangesAsync();

            var sut = new ProductsStore(_dbContext);

            var result = await sut.GetNonFreeProductsAsync();
        }
    }
}
