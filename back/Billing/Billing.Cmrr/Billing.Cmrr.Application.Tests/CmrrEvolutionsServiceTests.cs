using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Cmrr.Domain.Situation;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Cmrr.Application.Tests
{
    public class CmrrEvolutionsServiceTests
    {
        [Fact]
        public async Task ShouldThrowWhenStartPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 02);
            var endPeriod = new DateTime(2021, 01, 01);
            var evolutionFilter = new CmrrFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                Axis = CmrrAxis.Product
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();
            var contractAxisSectionsSituationsServiceMock = new Mock<IContractAxisSectionSituationsService>();

            var sut = new CmrrEvolutionsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, null, contractAxisSectionsSituationsServiceMock.Object, null);

            Func<Task<CmrrEvolution>> func = () => sut.GetEvolutionAsync(evolutionFilter);

            await func.Should().ThrowExactlyAsync<ArgumentException>().WithMessage("*date*");
        }

        [Fact]
        public async Task ShouldThrowWhenEndPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 01, 02);
            var evolutionFilter = new CmrrFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                Axis = CmrrAxis.Product
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();
            var contractAxisSectionsSituationsServiceMock = new Mock<IContractAxisSectionSituationsService>();

            var sut = new CmrrEvolutionsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, null, contractAxisSectionsSituationsServiceMock.Object, null);

            Func<Task<CmrrEvolution>> func = () => sut.GetEvolutionAsync(evolutionFilter);

            await func.Should().ThrowExactlyAsync<ArgumentException>().WithMessage("*date*");
        }

        [Fact]
        public async Task ShouldReturnEvolutionAsync()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 03, 01);
            var evolutionFilter = new CmrrFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                Axis = CmrrAxis.Product
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<AccessRight>())).ReturnsAsync(() =>
            {
                var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract { Id = 10, ProductId = 1 },
                    new CmrrContract { Id = 11, ProductId = 1 },
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var counts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod.AddMonths(-1),
                        ContractId = 10,
                        EuroTotal = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        EuroTotal = 20,
                        Id = 2
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod.AddMonths(1),
                        ContractId = 10,
                        EuroTotal = 5,
                        Id = 3
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        EuroTotal = 100,
                        Id = 4
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod.AddMonths(1),
                        ContractId = 11,
                        EuroTotal = 200,
                        Id = 5
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod.AddMonths(2),
                        ContractId = 11,
                        EuroTotal = 350,
                        Id = 6
                    },
                };

            cmrrCountsStoreMock.Setup(x => x.GetBetweenAsync(startPeriod.AddMonths(-1), endPeriod)).ReturnsAsync(() => counts);

            var productsStoreMock = new Mock<IProductsStore>();
            var productSolution = new ProductSolution
            {
                ProductId = 1,
                Product = new Product { Id = 1, Name = "Product", Family = new ProductFamily { Id = 1, Name = "Family", DisplayOrder = 1}},
                SolutionId = 1,
                Solution = new Solution { Id = 1, Name = "Solution" }
            };
            productSolution.Product.ProductSolutions = new EditableList<ProductSolution> { productSolution };
            productsStoreMock
                .Setup(p => p.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>()))
                .ReturnsAsync
                (
                    new List<Product>
                    {
                        productSolution.Product
                    }
                );

            var cmrrRightsFilterMock = new Mock<ICmrrRightsFilter>();
            cmrrRightsFilterMock.Setup(x => x.GetReadAccessAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(AccessRight.All);
            var sut = new CmrrEvolutionsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object, cmrrRightsFilterMock.Object, new ContractAxisSectionSituationsService(new BreakdownService(productsStoreMock.Object, new BreakDownInMemoryCache())), new ClaimsPrincipal());

            var cmrrEvolution = await sut.GetEvolutionAsync(evolutionFilter);

            cmrrEvolution.Should().NotBeNull();

            cmrrEvolution.StartPeriod.Should().Be(startPeriod);
            cmrrEvolution.EndPeriod.Should().Be(endPeriod);

            cmrrEvolution.Lines.Should().NotBeNullOrEmpty();
            cmrrEvolution.Lines.Should().HaveCount(3);

            var firstMonthLine = cmrrEvolution.Lines.Single(l => l.Period == startPeriod);
            firstMonthLine.Amount.Should().Be(100 + 20);
            firstMonthLine.Expansion.Should().Be(10);
            firstMonthLine.Creation.Should().Be(100);


            var secondMonthLine = cmrrEvolution.Lines.Single(l => l.Period == startPeriod.AddMonths(1));
            secondMonthLine.Amount.Should().Be(200 + 5);
            secondMonthLine.Expansion.Should().Be(100);
            secondMonthLine.Contraction.Should().Be(-15);

            var thirdMonthLine = cmrrEvolution.Lines.Single(l => l.Period == startPeriod.AddMonths(2));
            thirdMonthLine.Amount.Should().Be(350);
            thirdMonthLine.Expansion.Should().Be(150);
            thirdMonthLine.Contraction.Should().Be(-5);
        }
    }
}
