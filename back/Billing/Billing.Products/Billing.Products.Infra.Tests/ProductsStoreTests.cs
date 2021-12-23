using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Billing.Products.Infra.Storage;
using Billing.Products.Infra.Storage.Stores;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Billing.Products.Infra.Tests
{
    public class ProductsStoreTests
    {
        private readonly ProductDbContext _dbContext;

        public ProductsStoreTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<ProductDbContext>("products", o => new ProductDbContext(o));
        }
        [Fact]
        public async Task Should_Return_Non_Free_Product_Async()
        {
            var product1 = new Product { Id = 1, Name = "Name1", IsFreeUse = false, FamilyId = 1 };
            var product2 = new Product { Id = 2, Name = "Name2", IsFreeUse = false, FamilyId = 2};
            var product3 = new Product { Id = 3, Name = "Name3", IsFreeUse = true, FamilyId = 1 };


            var businessUnit100 = new BusinessUnit { Id = 100, Name = "100" };
            var businessUnit200 = new BusinessUnit { Id = 200, Name = "200" };

            var solution10 = new Solution { Id = 10, Name = "Solution1", BusinessUnitId= businessUnit100.Id };
            var solution20 = new Solution { Id = 11, Name = "Solution2", BusinessUnitId = businessUnit200.Id };

            var productSolution1 = new ProductSolution { ProductId = product1.Id, SolutionId = solution10.Id };
            var productSolution2 = new ProductSolution { ProductId = product1.Id, SolutionId = solution20.Id };
            var productSolution3 = new ProductSolution { ProductId = product2.Id, SolutionId = solution10.Id };


            var family1 = new ProductFamily { Id = 1, Name = "family1" };
            var family2 = new ProductFamily { Id = 2, Name = "family2" };

            await _dbContext.AddRangeAsync(new List<Product> { product1, product2, product3 });
            await _dbContext.AddRangeAsync(new List<BusinessUnit> { businessUnit100, businessUnit200 });
            await _dbContext.AddRangeAsync(new List<Solution> { solution10, solution20});
            await _dbContext.AddRangeAsync(new List<ProductFamily> { family1, family2 });
            await _dbContext.AddRangeAsync(new List<ProductSolution> { productSolution1, productSolution2, productSolution3 });

            await _dbContext.SaveChangesAsync();

            var sut = new ProductsStore(_dbContext);

            var result = await sut.GetAsync(new ProductsFilter { NonFreeOnly = true }, ProductsIncludes.All);

            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);

            var solutionsForProduct1 = result.First(p => p.Id == product1.Id).ProductSolutions.Select(ps => ps.Solution);
            solutionsForProduct1.Should().NotBeNullOrEmpty();
            solutionsForProduct1.Should().HaveCount(2);
            solutionsForProduct1.Select(s => s.Id).Should().Contain(new List<int>(2) { solution10.Id, solution20.Id });
            solutionsForProduct1.First(s => s.Id == solution10.Id).BusinessUnit.Id.Should().Be(businessUnit100.Id);
            solutionsForProduct1.First(s => s.Id == solution20.Id).BusinessUnit.Id.Should().Be(businessUnit200.Id);
        }
    }
}
