using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using FluentAssertions;
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
        [Fact]
        public void ShouldThrowWhenStartPeriodIsDefault()
        {
            var startPeriod = default(DateTime);
            var endPeriod = new DateTime(2021, 01, 01);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(situationFilter);

            func.Should().ThrowExactly<ArgumentNullException>().WithMessage("*startPeriod*");
        }

        [Fact]
        public void ShouldThrowWhenEndPeriodIsDefault()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = default(DateTime);
            var situationFilter = new CmrrSituationFilter
            {
                StartPeriod = startPeriod,
                EndPeriod = endPeriod
            };

            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();
            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(situationFilter);

            func.Should().ThrowExactly<ArgumentNullException>().WithMessage("*endPeriod*");
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

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(situationFilter);

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

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(situationFilter);

            func.Should().ThrowExactly<ArgumentException>().WithMessage("*endPeriod*");
        }

        [Fact]
        public async Task ShouldReturnContractSituationAsync()
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
                    new CmrrContract { Id = 10 },
                    new CmrrContract { Id = 11 }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        Id = 2
                    }
                };
                return cmrrCounts;
            });

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        Id = 3
                    }
                };
                return cmrrCounts;
            });

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);
    
            var cmrrContractSituations = await sut.GetContractSituationsAsync(situationFilter);

            // Assert
            cmrrContractSituations.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Should().HaveCount(2);

            var contract10 = cmrrContractSituations.First(s => s.EndPeriodCount != null);

            contract10.ContractId.Should().Be(10);
            contract10.EndPeriodCount.Id.Should().Be(3);
            contract10.StartPeriodCount.Id.Should().Be(1);

            var contract11 = cmrrContractSituations.First(s => s.EndPeriodCount is null);

            contract11.ContractId.Should().Be(11);
            contract11.EndPeriodCount.Should().BeNull();
            contract11.StartPeriodCount.Id.Should().Be(2);
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

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            var cmrrContractSituations = await sut.GetContractSituationsAsync(situationFilter);

            // Assert
            cmrrContractSituations.Should().NotBeNull();
            cmrrContractSituations.Should().HaveCount(0);
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
                    new CmrrContract { Id = 10, ClientId = 100 },
                    new CmrrContract { Id = 11, ClientId = 101 }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        Id = 2
                    }
                };
                return cmrrCounts;
            });

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        Id = 3
                    }
                };
                return cmrrCounts;
            });

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            var cmrrContractSituations = await sut.GetContractSituationsAsync(situationFilter);

            // Assert
            cmrrContractSituations.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Should().HaveCount(1);

            var contract10 = cmrrContractSituations.First(s => s.EndPeriodCount != null);

            contract10.ContractId.Should().Be(10);
            contract10.EndPeriodCount.Id.Should().Be(3);
            contract10.StartPeriodCount.Id.Should().Be(1);
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
                    new CmrrContract { Id = 10, DistributorId = "100" },
                    new CmrrContract { Id = 11, DistributorId = "101" }
                 };
                return cmrrContracts;
            });

            var cmrrCountsStoreMock = new Mock<ICmrrCountsStore>();

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(startPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 10,
                        Id = 1
                    },
                    new CmrrCount
                    {
                        CountPeriod = startPeriod,
                        ContractId = 11,
                        Id = 2
                    }
                };
                return cmrrCounts;
            });

            cmrrCountsStoreMock.Setup(x => x.GetByPeriodAsync(endPeriod)).ReturnsAsync(() =>
            {
                var cmrrCounts = new List<CmrrCount>
                {
                    new CmrrCount
                    {
                        CountPeriod = endPeriod,
                        ContractId = 10,
                        Id = 3
                    }
                };
                return cmrrCounts;
            });

            // Act
            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            var cmrrContractSituations = await sut.GetContractSituationsAsync(situationFilter);

            // Assert
            cmrrContractSituations.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Should().HaveCount(1);

            var contract10 = cmrrContractSituations.First(s => s.EndPeriodCount != null);

            contract10.ContractId.Should().Be(10);
            contract10.EndPeriodCount.Id.Should().Be(3);
            contract10.StartPeriodCount.Id.Should().Be(1);
        }
    }
}
