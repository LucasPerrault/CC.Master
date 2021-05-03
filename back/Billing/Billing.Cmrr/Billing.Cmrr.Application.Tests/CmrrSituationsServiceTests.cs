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

            var cmrrContractsStoreMock = GetCmrrContractStoreMock();
            var cmrrCountsStoreMock = GetCmrrCountsStoreMock(startPeriod, endPeriod);

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(startPeriod, endPeriod);

            func.Should().ThrowExactly<ArgumentNullException>().WithMessage("*startPeriod*");
        }

        [Fact]
        public void ShouldThrowWhenEndPeriodIsDefault()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = default(DateTime);

            var cmrrContractsStoreMock = GetCmrrContractStoreMock();
            var cmrrCountsStoreMock = GetCmrrCountsStoreMock(startPeriod, endPeriod);

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(startPeriod, endPeriod);

            func.Should().ThrowExactly<ArgumentNullException>().WithMessage("*endPeriod*");
        }

        [Fact]
        public void ShouldThrowWhenStartPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 02);
            var endPeriod = new DateTime(2021, 01, 01);

            var cmrrContractsStoreMock = GetCmrrContractStoreMock();
            var cmrrCountsStoreMock = GetCmrrCountsStoreMock(startPeriod, endPeriod);

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(startPeriod, endPeriod);

            func.Should().ThrowExactly<ArgumentException>().WithMessage("*startPeriod*");
        }

        [Fact]
        public void ShouldThrowWhenEndPeriodIsNotOnDayOneOfMonth()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 01, 02);

            var cmrrContractsStoreMock = GetCmrrContractStoreMock();
            var cmrrCountsStoreMock = GetCmrrCountsStoreMock(startPeriod, endPeriod);

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            Func<Task<List<CmrrContratSituation>>> func = () => sut.GetContractSituationsAsync(startPeriod, endPeriod);

            func.Should().ThrowExactly<ArgumentException>().WithMessage("*endPeriod*");
        }

        [Fact]
        public async Task ShouldReturnContractSituationAsync()
        {
            var startPeriod = new DateTime(2021, 01, 01);
            var endPeriod = new DateTime(2021, 02, 01);

            var cmrrContractsStoreMock = GetCmrrContractStoreMock();
            var cmrrCountsStoreMock = GetCmrrCountsStoreMock(startPeriod, endPeriod);

            var sut = new CmrrSituationsService(cmrrContractsStoreMock.Object, cmrrCountsStoreMock.Object);

            var cmrrContractSituations = await sut.GetContractSituationsAsync(startPeriod, endPeriod);

            cmrrContractSituations.Should().NotBeNullOrEmpty();
            cmrrContractSituations.Should().HaveCount(2);

            var contract10 = cmrrContractSituations.First(s => s.LastPeriodCount != null);

            contract10.ContractId.Should().Be(10);
            contract10.LastPeriodCount.Id.Should().Be(3);
            contract10.FirstPeriodCount.Id.Should().Be(1);

            var contract11 = cmrrContractSituations.First(s => s.LastPeriodCount is null);

            contract11.ContractId.Should().Be(11);
            contract11.LastPeriodCount.Should().BeNull();
            contract11.FirstPeriodCount.Id.Should().Be(2);
        }


        private Mock<ICmrrContractsStore> GetCmrrContractStoreMock()
        {
            var cmrrContractsStoreMock = new Mock<ICmrrContractsStore>();

            cmrrContractsStoreMock.Setup(x => x.GetContractsNotEndedAtAsync(It.IsAny<DateTime>())).ReturnsAsync(() =>
             {
                 var cmrrContracts = new List<CmrrContract>
                 {
                    new CmrrContract
                    {
                        Id=10,
                    },
                    new CmrrContract
                    {
                        Id = 11
                    }
                 };

                 return cmrrContracts;
             });

            return cmrrContractsStoreMock;
        }

        private Mock<ICmrrCountsStore> GetCmrrCountsStoreMock(DateTime startPeriod, DateTime endPeriod)
        {
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

            return cmrrCountsStoreMock;
        }
    }
}
