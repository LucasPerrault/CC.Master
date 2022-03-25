using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Situation;
using Billing.Products.Domain;
using Billing.Products.Infra.Storage;
using Billing.Products.Infra.Storage.Stores;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Billing.Cmrr.Application.Tests
{
    public class BreakdownServiceTests
    {
        private readonly ProductDbContext _dbContext;

        public BreakdownServiceTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<ProductDbContext>("products", o => new ProductDbContext(o));
        }

        [Fact]
        public async Task ShouldBreakDownProductAxis()
        {
            var figgoFamily = new ProductFamily { Id = 1, Name = "figgo family" };
            var cleemyFamily = new ProductFamily { Id = 2, Name = "cleemy family" };
            var suiteFamily = new ProductFamily { Id = 3, Name = "suite family" };

            var figgoBu = new BusinessUnit { Id = 100, Name="Figgo BU" };
            var cleemyBu = new BusinessUnit { Id = 101, Name="Cleemy BU" };

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnit = figgoBu };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnit = cleemyBu };

            var figgo = new Product
            {
                Id = 1,
                Name = "figgo",
                Family = figgoFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionFiggo, Share = 1 }}
            };
            var cleemy = new Product
            {
                Id = 2,
                Name = "cleemy",
                Family = cleemyFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionCleemy, Share = 1 }}

            };
            var suite = new Product
            {
                Id = 3,
                Name = "suite",
                Family = suiteFamily,
                ProductSolutions = new List<ProductSolution>
                {
                    new ProductSolution { Solution = solutionFiggo, Share = 1 },
                    new ProductSolution { Solution = solutionCleemy, Share = 1 },
                }
            };

            await _dbContext.AddAsync(figgo);
            await _dbContext.AddAsync(cleemy);
            await _dbContext.AddAsync(suite);
            await _dbContext.SaveChangesAsync();

            var service = new BreakdownService(new ProductsStore(_dbContext), new BreakDownInMemoryCache());
            var allBreakdowns = await service.GetBreakdownsAsync(CmrrAxis.Product);

            allBreakdowns.Should().HaveCount(4);

            var figgoBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == figgo.Id);
            figgoBreakdowns.Should().HaveCount(1);
            figgoBreakdowns.Single(b => b.SubSection == figgo.Name).Ratio.Should().Be(1m);

            var cleemyBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == cleemy.Id);
            cleemyBreakdowns.Should().HaveCount(1);
            cleemyBreakdowns.Single(b => b.SubSection == cleemy.Name).Ratio.Should().Be(1m);

            var suiteBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == suite.Id);
            suiteBreakdowns.Should().HaveCount(2);
            foreach (var b in suiteBreakdowns)
            {
                b.Ratio.Should().Be(0.5m);
            }
        }

        [Fact]
        public async Task ShouldBreakDownProductAxisWithThreeSolutions()
        {
            var suiteFamily = new ProductFamily { Id = 3, Name = "suite family" };

            var figgoBu = new BusinessUnit { Id = 100, Name="Figgo BU" };
            var cleemyBu = new BusinessUnit { Id = 101, Name="Cleemy BU" };

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnit = figgoBu };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnit = cleemyBu };
            var solutionCleemyProc = new Solution { Id = 12, Name = "Solution Cleemy Proc", BusinessUnit = cleemyBu };

            var suite = new Product
            {
                Id = 3,
                Name = "suite",
                Family = suiteFamily,
                ProductSolutions = new List<ProductSolution>
                {
                    new ProductSolution { Solution = solutionFiggo, Share = 1 },
                    new ProductSolution { Solution = solutionCleemy, Share = 1 },
                    new ProductSolution { Solution = solutionCleemyProc, Share = 1 },
                }
            };

            await _dbContext.AddAsync(suite);
            await _dbContext.SaveChangesAsync();

            var service = new BreakdownService(new ProductsStore(_dbContext), new BreakDownInMemoryCache());
            var allBreakdowns = await service.GetBreakdownsAsync(CmrrAxis.Product);

            allBreakdowns.Should().HaveCount(3);

            var suiteBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == suite.Id);
            suiteBreakdowns.Should().HaveCount(3);
            foreach (var b in suiteBreakdowns)
            {
                b.Ratio.Should().BeApproximately(0.333m, 0.001m);
            }
        }

        [Fact]
        public async Task ShouldBreakDownProductAxisAccordingToShare()
        {
            var suiteFamily = new ProductFamily { Id = 3, Name = "suite family" };

            var figgoBu = new BusinessUnit { Id = 100, Name = "Figgo BU" };
            var cleemyBu = new BusinessUnit { Id = 101, Name = "Cleemy BU" };

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnit = figgoBu };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnit = cleemyBu };
            var solutionCleemyProc = new Solution { Id = 12, Name = "Solution Cleemy Proc", BusinessUnit = cleemyBu };

            var suite = new Product
            {
                Id = 3,
                Name = "suite",
                Family = suiteFamily,
                ProductSolutions = new List<ProductSolution>
                {
                    new ProductSolution { Solution = solutionFiggo, Share = 100 },
                    new ProductSolution { Solution = solutionCleemy, Share = 10 },
                    new ProductSolution { Solution = solutionCleemyProc, Share = 1 },
                }
            };

            var totalShare = 100 + 10 + 1;

            await _dbContext.AddAsync(suite);
            await _dbContext.SaveChangesAsync();

            var service = new BreakdownService(new ProductsStore(_dbContext), new BreakDownInMemoryCache());
            var allBreakdowns = await service.GetBreakdownsAsync(CmrrAxis.Product);

            allBreakdowns.Should().HaveCount(3);

            var suiteBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == suite.Id);
            suiteBreakdowns.Should().HaveCount(3);
            var expectedRatios = new List<decimal> { 100m / totalShare, 10m / totalShare, 1m / totalShare };
            suiteBreakdowns.Select(b => b.Ratio).Should().BeEquivalentTo(expectedRatios);
        }

        [Fact]
        public async Task ShouldBreakDownBusinessUnitAxis()
        {
            var figgoFamily = new ProductFamily { Id = 1, Name = "figgo family" };
            var cleemyFamily = new ProductFamily { Id = 2, Name = "cleemy family" };
            var suiteFamily = new ProductFamily { Id = 3, Name = "suite family" };

            var figgoBu = new BusinessUnit { Id = 100, Name="Figgo BU" };
            var cleemyBu = new BusinessUnit { Id = 101, Name="Cleemy BU" };

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnit = figgoBu };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnit = cleemyBu };

            var figgo = new Product
            {
                Id = 1,
                Name = "figgo",
                Family = figgoFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionFiggo, Share = 1 }}
            };
            var cleemy = new Product
            {
                Id = 2,
                Name = "cleemy",
                Family = cleemyFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionCleemy, Share = 1 }}

            };
            var suite = new Product
            {
                Id = 3,
                Name = "suite",
                Family = suiteFamily,
                ProductSolutions = new List<ProductSolution>
                {
                    new ProductSolution { Solution = solutionFiggo, Share = 1 },
                    new ProductSolution { Solution = solutionCleemy, Share = 1 },
                }
            };

            await _dbContext.AddAsync(figgo);
            await _dbContext.AddAsync(cleemy);
            await _dbContext.AddAsync(suite);
            await _dbContext.SaveChangesAsync();

            var service = new BreakdownService(new ProductsStore(_dbContext), new BreakDownInMemoryCache());
            var allBreakdowns = await service.GetBreakdownsAsync(CmrrAxis.BusinessUnit);

            allBreakdowns.Should().HaveCount(4);

            var cleemyBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == cleemyBu.Id);
            cleemyBreakdowns.Should().HaveCount(2);
            cleemyBreakdowns.Single(b => b.ProductId == suite.Id).Ratio.Should().Be(0.5m);
            cleemyBreakdowns.Single(b => b.ProductId == cleemy.Id).Ratio.Should().Be(1m);

            var figgoBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == figgoBu.Id);
            figgoBreakdowns.Should().HaveCount(2);
            figgoBreakdowns.Single(b => b.ProductId == suite.Id).Ratio.Should().Be(0.5m);
            figgoBreakdowns.Single(b => b.ProductId == figgo.Id).Ratio.Should().Be(1m);
        }

        [Fact]
        public async Task ShouldBreakDownBusinessUnitAxisAccordingToShare()
        {
            var figgoFamily = new ProductFamily { Id = 1, Name = "figgo family" };
            var cleemyFamily = new ProductFamily { Id = 2, Name = "cleemy family" };
            var suiteFamily = new ProductFamily { Id = 3, Name = "suite family" };

            var figgoBu = new BusinessUnit { Id = 100, Name = "Figgo BU" };
            var cleemyBu = new BusinessUnit { Id = 101, Name = "Cleemy BU" };

            var solutionFiggo = new Solution { Id = 10, Name = "Solution Figgo", BusinessUnit = figgoBu };
            var solutionCleemy = new Solution { Id = 11, Name = "Solution Cleemy", BusinessUnit = cleemyBu };

            var figgo = new Product
            {
                Id = 1,
                Name = "figgo",
                Family = figgoFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionFiggo, Share = 1 } }
            };
            var cleemy = new Product
            {
                Id = 2,
                Name = "cleemy",
                Family = cleemyFamily,
                ProductSolutions = new List<ProductSolution> { new ProductSolution { Solution = solutionCleemy, Share = 1 } }

            };
            var suite = new Product
            {
                Id = 3,
                Name = "suite",
                Family = suiteFamily,
                ProductSolutions = new List<ProductSolution>
                {
                    new ProductSolution { Solution = solutionFiggo, Share = 10 },
                    new ProductSolution { Solution = solutionCleemy, Share = 1 },
                }
            };

            await _dbContext.AddAsync(figgo);
            await _dbContext.AddAsync(cleemy);
            await _dbContext.AddAsync(suite);
            await _dbContext.SaveChangesAsync();

            var service = new BreakdownService(new ProductsStore(_dbContext), new BreakDownInMemoryCache());
            var allBreakdowns = await service.GetBreakdownsAsync(CmrrAxis.BusinessUnit);

            allBreakdowns.Should().HaveCount(4);

            var cleemyBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == cleemyBu.Id);
            cleemyBreakdowns.Should().HaveCount(2);
            cleemyBreakdowns.Single(b => b.ProductId == suite.Id).Ratio.Should().Be(1m/11);
            cleemyBreakdowns.Single(b => b.ProductId == cleemy.Id).Ratio.Should().Be(1m);

            var figgoBreakdowns = allBreakdowns.Where(b => b.AxisSection.Id == figgoBu.Id);
            figgoBreakdowns.Should().HaveCount(2);
            figgoBreakdowns.Single(b => b.ProductId == suite.Id).Ratio.Should().Be(10m / 11);
            figgoBreakdowns.Single(b => b.ProductId == figgo.Id).Ratio.Should().Be(1m);
        }
    }
}
