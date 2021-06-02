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
        public async Task ShouldGetOrderedSituationsGroupedPerProductAsync()
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

        [Fact]
        public async Task ShouldGetOrderedSituationsGroupedPerSolutionAsync()
        {
            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            var product2 = new Product { Id = 2, Name = "cleemy", FamilyId = 2 };
            var product3 = new Product { Id = 3, Name = "figgo 2", FamilyId = 3 };
            await _dbContext.AddAsync(product);
            await _dbContext.AddAsync(product2);
            await _dbContext.AddAsync(product3);

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo" };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy" };
            await _dbContext.AddAsync(solutionFiggo);
            await _dbContext.AddAsync(solutionCleemy);

            await _dbContext.AddAsync(new ProductSolution { ProductId = 1, SolutionId = 10 });
            await _dbContext.AddAsync(new ProductSolution { ProductId = 2, SolutionId = 11 });
            await _dbContext.AddAsync(new ProductSolution { ProductId = 3, SolutionId = 10 });

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            var family2 = new ProductFamily { Id = 2, Name = "cleemy family" };
            var family3 = new ProductFamily { Id = 3, Name = "figgo family 2 " };
            await _dbContext.AddAsync(family);
            await _dbContext.AddAsync(family2);
            await _dbContext.AddAsync(family3);

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
                ProductId = 3,
                EndReason = ContractEndReason.Modification
            };

            var contractSituations = new List<CmrrContractSituation>
            {
                new CmrrContractSituation(contract1, null, null),
                new CmrrContractSituation(contract2, null, null),
                new CmrrContractSituation(contract3, null, null),
            };


            var sut = new ContractAnalyticSituationsService(new ProductsStore(_dbContext));

            var situations = await sut.GetOrderedSituationsAsync(CmrrAxis.Solution, contractSituations);

            situations.Should().HaveCount(2);
            situations.First(s => s.Key.Name == solutionFiggo.Name).Should().HaveCount(2);
            situations.First(s => s.Key.Name == solutionCleemy.Name).Should().HaveCount(1);
        }

        [Fact]
        public async Task ShouldGetOrderedSituationsGroupedPerBusinessUnitAsync()
        {
            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            var product2 = new Product { Id = 2, Name = "cleemy", FamilyId = 2 };
            var product3 = new Product { Id = 3, Name = "figgo 2", FamilyId = 3 };
            await _dbContext.AddAsync(product);
            await _dbContext.AddAsync(product2);
            await _dbContext.AddAsync(product3);

            var businessUnit100 = new BusinessUnit { Id = 100, Name="bu100" };
            var businessUnit101 = new BusinessUnit { Id = 101, Name="bu101" };
            await _dbContext.AddAsync(businessUnit100);
            await _dbContext.AddAsync(businessUnit101);

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnitId = businessUnit100.Id };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnitId = businessUnit101.Id };
            await _dbContext.AddAsync(solutionFiggo);
            await _dbContext.AddAsync(solutionCleemy);

            await _dbContext.AddAsync(new ProductSolution { ProductId = product.Id, SolutionId = solutionFiggo.Id });
            await _dbContext.AddAsync(new ProductSolution { ProductId = product2.Id, SolutionId = solutionCleemy.Id });
            await _dbContext.AddAsync(new ProductSolution { ProductId = product3.Id, SolutionId = solutionFiggo.Id });

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            var family2 = new ProductFamily { Id = 2, Name = "cleemy family" };
            var family3 = new ProductFamily { Id = 3, Name = "figgo family 2 " };
            await _dbContext.AddAsync(family);
            await _dbContext.AddAsync(family2);
            await _dbContext.AddAsync(family3);

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
                ProductId = 3,
                EndReason = ContractEndReason.Modification
            };

            var contractSituations = new List<CmrrContractSituation>
            {
                new CmrrContractSituation(contract1, null, null),
                new CmrrContractSituation(contract2, null, null),
                new CmrrContractSituation(contract3, null, null),
            };


            var sut = new ContractAnalyticSituationsService(new ProductsStore(_dbContext));

            var situations = await sut.GetOrderedSituationsAsync(CmrrAxis.BusinessUnit, contractSituations);

            situations.Should().HaveCount(2);
            situations.First(s => s.Key.Name == businessUnit100.Name).Should().HaveCount(2);
            situations.First(s => s.Key.Name == businessUnit101.Name).Should().HaveCount(1);
        }
    }
}
