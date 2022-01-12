using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.CreationStrategy;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Environments;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class CountServiceTests
    {
        private readonly RemoteServiceMock _remoteServiceMock;
        private readonly Mock<IEnvironmentGroupStore> _envGroupStoreMock;
        private readonly CountCreationStrategyService _countStrategyService;
        private readonly Mock<ICountsStore> _countsStoreMock;
        private readonly Mock<IContractPricingsStore> _contractPricingsStore;
        private readonly Mock<ITimeProvider> _timeMock;

        public CountServiceTests()
        {
            _remoteServiceMock = new RemoteServiceMock();
            _envGroupStoreMock = new Mock<IEnvironmentGroupStore>(MockBehavior.Strict);
            _countsStoreMock = new Mock<ICountsStore>(MockBehavior.Strict);
            _contractPricingsStore = new Mock<IContractPricingsStore>(MockBehavior.Strict);
            _timeMock = new Mock<ITimeProvider>(MockBehavior.Strict);
            _countStrategyService = new CountCreationStrategyService
            (
                new PriceListService(),
                new MinimalBillingService(),
                _timeMock.Object,
                _countsStoreMock.Object,
                _contractPricingsStore.Object
            );
        }

        [Fact]
        public async Task ShouldFailForContractWithNoEnvironmentGroup()
        {
            _timeMock.Setup(t => t.Today()).Returns(new DateTime(2020, 05, 05));
            var service = new CountService(_remoteServiceMock, _envGroupStoreMock.Object, _countStrategyService);
            var contractsToCount = new List<Contract> { new Contract { Id = 123 } };
            await Assert.ThrowsAsync<ApplicationException>(() => service.CreateForPeriodAsync(new AccountingPeriod(2010, 01), contractsToCount));
        }

        [Fact]
        public async Task ShouldFailIfOneContractCannotBeFoundInGroup()
        {
            var contract = new Contract { Id = 123, Environment = new ContractEnvironment { GroupId = 1} };
            var envGroup = new EnvironmentWithContractGroup { EnvironmentGroupId = 1, Contracts = new List<Contract>() };

            _envGroupStoreMock.SetupEnvGroups(1, envGroup);

            var service = new CountService(_remoteServiceMock, _envGroupStoreMock.Object, _countStrategyService);
            var contractsToCount = new List<Contract> { contract };

            await Assert.ThrowsAsync<ApplicationException>(() => service.CreateForPeriodAsync(new AccountingPeriod(2010, 01), contractsToCount));
        }

        [Fact]
        public async Task ShouldCountSimpleContract()
        {
            _timeMock.Setup(t => t.Today()).Returns(new DateTime(2020, 05, 05));

            var contract = new TestContract
            {
                Id = 123,
                EnvGroupId = 1,
                PricingMethod = PricingMethod.Linear,
                PriceRows = new List<PriceRow>
                {
                    new PriceRow { MaxIncludedCount = 500000, FixedPrice = 3, UnitPrice = 7 },
                },
            }.ToContract();

            var contractWithCountNumber = new ContractWithCountNumber { Contract = contract, Period = new AccountingPeriod(2010, 01), CountNumber = 1000 };
            _remoteServiceMock.CountContextMock.SetupRemoteCounts(contract, contractWithCountNumber);

            var envGroup = new EnvironmentWithContractGroup
            {
                EnvironmentGroupId = 1,
                Contracts = new List<Contract> { contract },
            };
            _envGroupStoreMock.SetupEnvGroups(1, envGroup);

            var service = new CountService(_remoteServiceMock, _envGroupStoreMock.Object, _countStrategyService);
            var contractsToCount = new List<Contract> { contract };

            var counts = await service.CreateForPeriodAsync(new AccountingPeriod(2010, 01), contractsToCount);
            var count = counts.Single();
            count.Number.Should().Be(1000);
            count.CountPeriod.Should().Be(new AccountingPeriod(2010, 01));
            count.ContractId.Should().Be(123);
            count.FixedPrice.Should().Be(3);
            count.UnitPrice.Should().Be(7);
            count.IsMinimalBilling.Should().Be(false);
            count.TotalInCurrency.Should().Be(3 + 1000 * 7);
        }

        [Fact]
        public async Task ShouldCountConstantContract()
        {
            _timeMock.Setup(t => t.Today()).Returns(new DateTime(2020, 05, 05));

            var contract = new TestContract
            {
                Id = 123,
                EnvGroupId = 1,
                PricingMethod = PricingMethod.Constant,
            }.ToContract();

            var contractWithCountNumber = new ContractWithCountNumber { Contract = contract, Period = new AccountingPeriod(2010, 01), CountNumber = 1000 };
            _remoteServiceMock.CountContextMock.SetupRemoteCounts(contract, contractWithCountNumber);

            _contractPricingsStore.SetupGetAsync(new ContractPricing { ContractId = 123, PricingMethod = PricingMethod.Constant , ConstantPrice = 1337});

            var envGroup = new EnvironmentWithContractGroup
            {
                EnvironmentGroupId = 1,
                Contracts = new List<Contract> { contract },
            };
            _envGroupStoreMock.SetupEnvGroups(1, envGroup);

            var service = new CountService(_remoteServiceMock, _envGroupStoreMock.Object, _countStrategyService);
            var contractsToCount = new List<Contract> { contract };

            var counts = await service.CreateForPeriodAsync(new AccountingPeriod(2010, 01), contractsToCount);
            var count = counts.Single();
            count.Number.Should().Be(1000);
            count.CountPeriod.Should().Be(new AccountingPeriod(2010, 01));
            count.ContractId.Should().Be(123);
            count.FixedPrice.Should().Be(1337);
            count.UnitPrice.Should().Be(0);
            count.IsMinimalBilling.Should().Be(false);
            count.TotalInCurrency.Should().Be(1337);
        }

        [Fact]
        public async Task ShouldCountContractWithContractOfSameProduct()
        {
            _timeMock.Setup(t => t.Today()).Returns(new DateTime(2020, 05, 05));

            var contract = new TestContract
            {
                Id = 123,
                EnvGroupId = 1,
                PricingMethod = PricingMethod.Linear,
                PriceRows = new List<PriceRow>
                {
                    new PriceRow { MaxIncludedCount = 15, FixedPrice = 3, UnitPrice = 7 },
                    new PriceRow { MaxIncludedCount = 2000, FixedPrice = 0, UnitPrice = 0.5m }
                },
            }.ToContract();

            var otherContract = new TestContract { Id = 124, EnvGroupId = 1, PriceRows = new List<PriceRow>() }.ToContract();

            var contractWithCountNumber = new ContractWithCountNumber { Contract = contract, Period = new AccountingPeriod(2010, 01), CountNumber = 15 };
            _remoteServiceMock.CountContextMock.SetupRemoteCounts(contract, contractWithCountNumber);

            var otherContractWithCountNumber = new ContractWithCountNumber { Contract = otherContract, Period = new AccountingPeriod(2010, 01), CountNumber = 1000 };
            _remoteServiceMock.CountContextMock.SetupRemoteCounts(otherContract, otherContractWithCountNumber);

            var envGroup = new EnvironmentWithContractGroup
            {
                EnvironmentGroupId = 1,
                Contracts = new List<Contract> { contract, otherContract },
            };
            _envGroupStoreMock.SetupEnvGroups(1, envGroup);
            var service = new CountService(_remoteServiceMock, _envGroupStoreMock.Object, _countStrategyService);

            var oneContract = new List<Contract> { contract };

            var oneContractCounts = await service.CreateForPeriodAsync(new AccountingPeriod(2010, 01), oneContract);
            var oneContractCount = oneContractCounts.Single();
            oneContractCount.Number.Should().Be(15);
            oneContractCount.CountPeriod.Should().Be(new AccountingPeriod(2010, 01));
            oneContractCount.ContractId.Should().Be(123);
            oneContractCount.FixedPrice.Should().Be(0);
            oneContractCount.UnitPrice.Should().Be((decimal)0.5);
            oneContractCount.IsMinimalBilling.Should().Be(false);
            oneContractCount.TotalInCurrency.Should().Be((decimal)(15 * 0.5));
        }

        private class RemoteServiceMock : ICountRemoteService
        {
            public readonly Mock<ICountContext> CountContextMock = new Mock<ICountContext>(MockBehavior.Strict);

            public RemoteServiceMock()
            {
                CountContextMock.Setup(c => c.Dispose());
            }
            public ICountContext GetCountContext()
            {
                return CountContextMock.Object;
            }
        }

        private class TestContract
        {
            public int Id { get; set; }
            public int EnvGroupId { get; set; }
            public PricingMethod PricingMethod { get; set; }
            public List<PriceRow> PriceRows { get; set; }

            public Contract ToContract()
            {
                return new Contract
                {
                    Id = Id,
                    Environment = new ContractEnvironment { GroupId = EnvGroupId },
                    CommercialOffer = new CommercialOffer
                    {
                        PricingMethod = PricingMethod,
                        PriceLists = new List<PriceList>
                        {
                            new PriceList { StartsOn = new DateTime(2000), Rows = PriceRows }
                        },
                    },
                };
            }
        }
    }


    internal static class TestsExtensions
    {
        public static void SetupRemoteCounts(this Mock<ICountContext> countContext, Contract contract, ContractWithCountNumber remoteCount)
            => countContext
                .Setup(s => s.GetNumberFromRemoteAsync(contract, new AccountingPeriod(2010, 01)))
                .ReturnsAsync(remoteCount);

        public static void SetupEnvGroups(this Mock<IEnvironmentGroupStore> store, HashSet<int> ints, params EnvironmentWithContractGroup[] envGroups)
            => store
                .Setup(s => s.GetEnvGroupsAsync(It.Is<IEnumerable<int>>(e => e.Count() == ints.Count() && !e.Except(ints).Any())))
                .ReturnsAsync(envGroups?.ToList() ?? new List<EnvironmentWithContractGroup>());

        public static void SetupEnvGroups(this Mock<IEnvironmentGroupStore> store, int groupId, params EnvironmentWithContractGroup[] envGroups)
            => SetupEnvGroups(store, new HashSet<int> { groupId }, envGroups);

        public static void SetupGetAsync(this Mock<IContractPricingsStore> store, params ContractPricing[] pricings)
            => store
                .Setup(s => s.GetAsync())
                .ReturnsAsync(pricings?.ToList() ?? new List<ContractPricing>());
    }
}
