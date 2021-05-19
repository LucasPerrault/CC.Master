using Billing.Cmrr.Domain;
using Billing.Products.Domain;
using Billing.Products.Infra.Storage;
using Billing.Products.Infra.Storage.Stores;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Cmrr.Application.Tests
{
    public class ContractAnalyticSituationsServiceTests
    {
        private readonly ProductDbContext _dbContext;

        public ContractAnalyticSituationsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _dbContext = new ProductDbContext(options);
        }

        [Fact]
        public async Task ShouldFilterContractSituationOnDistributorIdsAsync()
        {
            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            var product2 = new Product { Id = 2, Name = "cleemy", FamilyId = 2 };
            await _dbContext.AddAsync(product);
            await _dbContext.AddAsync(product2);

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            var family2 = new ProductFamily { Id = 2, Name = "cleemy family" };
            await _dbContext.AddAsync(family);
            await _dbContext.AddAsync(family2);
            await _dbContext.SaveChangesAsync();

            var contract1 = new CmrrContract
            {
                ProductId = 1,
                EndReason = ContractEndReason.Modification
            };
            var contract2 = new CmrrContract
            {
                ProductId = 2,
                EndReason = ContractEndReason.Modification
            };
            var contract3 = new CmrrContract
            {
                ProductId = 1,
                EndReason = ContractEndReason.Modification
            };

            var contractSituations = new List<CmrrContractSituation>
            {
                new CmrrContractSituation(contract1, null, null),
                new CmrrContractSituation(contract2, null, null),
                new CmrrContractSituation(contract3, null, null),
            };


            var sut = new ContractAnalyticSituationsService(new ProductsStore(_dbContext));

            var situations = await sut.GetOrderedSituationsAsync(CmrrAxis.Product, contractSituations);

            situations.Should().HaveCount(2);
            situations.First(s => s.Key.Name == family.Name).Should().HaveCount(2);
            situations.First(s => s.Key.Name == family2.Name).Should().HaveCount(1);
        }
    }
}
