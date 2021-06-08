using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Products.Domain;
using Billing.Products.Infra.Storage;
using Billing.Products.Infra.Storage.Stores;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Cmrr.Application.Tests
{
    public class CmrrSituationsServiceTests
    {
        private readonly ProductDbContext _dbContext;

        public CmrrSituationsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _dbContext = new ProductDbContext(options);
        }

        [Fact]
        public void ShouldThrowWhenStartPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 02);
            var endPeriod = new DateTime(2021, 01, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, null);

            Func<Task<CmrrSituation>> func = () => sut.GetSituationAsync(situationFilter);

            func.Should().ThrowExactly<ArgumentException>().WithMessage("*startPeriod*");
        }

        [Fact]
        public void ShouldThrowWhenEndPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 01, 02);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, null);

            Func<Task<CmrrSituation>> func = () => sut.GetSituationAsync(situationFilter);

            func.Should().ThrowExactly<ArgumentException>().WithMessage("*endPeriod*");
        }

        [Fact]
        public async Task ShouldReturnSituationAsync()
        {
            // Arrange
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 02, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(() =>
            {
                var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract { Id = 10, ProductId = 1 },
                    new CmrrContract { Id = 11, ProductId = 1}
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();
            var startCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        EuroTotal = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        EuroTotal = 11,
                        Id = 2
                    }
                };
            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() => startCmrrCounts);

            var endCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        EuroTotal = 110,
                        Id = 3
                    }
                };

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() => endCmrrCounts);

            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            await _dbContext.AddAsync(product);

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            await _dbContext.AddAsync(family);
            await _dbContext.SaveChangesAsync();
            var productStore = new ProductsStore(_dbContext);
            var contractAxisSectionSituationsService = new ContractAxisSectionSituationsService(productStore, new BreakdownService(productStore));

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, contractAxisSectionSituationsService);

            var cmrrContractSituations = await sut.GetSituationAsync(situationFilter);

            // Assert
            cmrrContractSituations.Lines.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Lines.Should().HaveCount(1);

            var line = cmrrContractSituations.Lines.First(s => s.Name == family.Name);
            Assert.Single(line.SubSections);
            var section = line.SubSections.Single(s => s.Key == product.Name).Value;

            section.TotalFrom.Amount.Should().Be(startCmrrCounts.Sum(c => c.EuroTotal));
            section.TotalTo.Amount.Should().Be(endCmrrCounts.Sum(c => c.EuroTotal));

            var startCountForContract10 = startCmrrCounts.First(c => c.ContractId == 10);
            var endCountForContract10 = endCmrrCounts.First(c => c.ContractId == 10);
            var cmrrContractSituation10 = section.Expansion.Top.First(c => c.Contract.Id == 10);

            section.Expansion.Amount.Should().Be(endCountForContract10.EuroTotal - startCountForContract10.EuroTotal);
            cmrrContractSituation10.Contract.Id.Should().Be(10);
            cmrrContractSituation10.EndPeriodCount.Id.Should().Be(3);
            cmrrContractSituation10.StartPeriodCount.Id.Should().Be(1);

            var startCountForContract11 = startCmrrCounts.First(c => c.ContractId == 11);
            var cmrrContractSituation11 = section.Termination.Top.First(c => c.Contract.Id == 11);

            section.Termination.Amount.Should().Be(-startCountForContract11.EuroTotal);
            cmrrContractSituation11.Contract.Id.Should().Be(11);
            cmrrContractSituation11.EndPeriodCount.Id.Should().BeNull();
            cmrrContractSituation11.StartPeriodCount.Id.Should().Be(2);
        }

        [Fact]
        public async Task ShouldNotReturnContractWithNoCountForStartAndEndPeriodAsync()
        {
            // Arrange
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 02, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(() =>
            {
                var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract { Id = 10 }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<CmrrCount>());

            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            await _dbContext.AddAsync(product);

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            await _dbContext.AddAsync(family);
            await _dbContext.SaveChangesAsync();
            var productStore = new ProductsStore(_dbContext);
            var contractAxisSectionSituationsService = new ContractAxisSectionSituationsService(productStore, new BreakdownService(productStore));

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, contractAxisSectionSituationsService);

            var cmrrContractSituations = await sut.GetSituationAsync(situationFilter);

            // Assert
            cmrrContractSituations.Should().NotBeNull();
            cmrrContractSituations.Lines.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldFilterContractSituationOnClientIdsAsync()
        {
            // Arrange
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 02, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                ClientId = new HashSet<int> { 100 }
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(() =>
            {
                var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract { Id = 10, ClientId = 100, ProductId = 1},
                    new CmrrContract { Id = 11, ClientId = 101, ProductId = 1 }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var startCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        EuroTotal = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        EuroTotal = 11,
                        Id = 2
                    }
                };
            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() => startCmrrCounts);

            var endCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        EuroTotal = 110,
                        Id = 3
                    }
                };

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() => endCmrrCounts);

            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            await _dbContext.AddAsync(product);

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            await _dbContext.AddAsync(family);
            await _dbContext.SaveChangesAsync();
            var productStore = new ProductsStore(_dbContext);
            var contractAxisSectionSituationsService = new ContractAxisSectionSituationsService(productStore, new BreakdownService(productStore));

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, contractAxisSectionSituationsService);

            var cmrrContractSituations = await sut.GetSituationAsync(situationFilter);

            // Assert
            cmrrContractSituations.Lines.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Lines.Should().HaveCount(1);

            var line = cmrrContractSituations.Lines.First(s => s.Name == family.Name);
            Assert.Single(line.SubSections);
            var section = line.SubSections.Single(s => s.Key == product.Name).Value;

            section.Termination.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Retraction.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Upsell.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Expansion.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Creation.Top.Should().NotContain(c => c.Contract.Id == 11);
        }

        [Fact]
        public async Task ShouldFilterContractSituationOnDistributorIdsAsync()
        {
            // Arrange
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 02, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                DistributorsId = new HashSet<string> { "100" }
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(() =>
            {
                var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract { Id = 10, DistributorId = "100", ProductId = 1 },
                    new CmrrContract { Id = 11, DistributorId = "101", ProductId = 1 }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();


            var startCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        EuroTotal = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        EuroTotal = 11,
                        Id = 2
                    }
                };
            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() => startCmrrCounts);

            var endCmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        EuroTotal = 110,
                        Id = 3
                    }
                };

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() => endCmrrCounts);

            var product = new Product { Id = 1, Name = "figgo", FamilyId = 1 };
            await _dbContext.AddAsync(product);

            var family = new ProductFamily { Id = 1, Name = "figgo family" };
            await _dbContext.AddAsync(family);
            await _dbContext.SaveChangesAsync();
            var productStore = new ProductsStore(_dbContext);
            var contractAxisSectionSituationsService = new ContractAxisSectionSituationsService(productStore, new BreakdownService(productStore));

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, contractAxisSectionSituationsService);

            var cmrrContractSituations = await sut.GetSituationAsync(situationFilter);

            // Assert
            cmrrContractSituations.Lines.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Lines.Should().HaveCount(1);

            var line = cmrrContractSituations.Lines.First(s => s.Name == family.Name);
            Assert.Single(line.SubSections);
            var section = line.SubSections.Single(s => s.Key == product.Name).Value;

            section.Termination.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Retraction.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Upsell.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Expansion.Top.Should().NotContain(c => c.Contract.Id == 11);
            section.Creation.Top.Should().NotContain(c => c.Contract.Id == 11);
        }
    }
}
