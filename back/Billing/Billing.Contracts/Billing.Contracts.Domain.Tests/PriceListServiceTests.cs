using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Offers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class PriceListServiceTests
    {
        [Theory]
        [InlineData(2002, 01)]
        [InlineData(2012, 01)]
        [InlineData(2022, 01)]
        [InlineData(2022, 02)]
        [InlineData(9999, 01)]
        public void ShouldGetSingleList(int year, int month)
        {
            var priceLists = new List<PriceList> { new PriceList { StartsOn = new DateTime(2002, 01, 01) } };
            var contract = new Contract { CommercialOffer = new CommercialOffer { PriceLists = priceLists } };

            new PriceListService()
                .GetPriceListForCount(contract, new DateTime(year, month, 01))
                .Should()
                .Be(priceLists.Single());
        }
        [Theory]
        [InlineData(2019, 01, 2021, 12, 2002)]
        [InlineData(2019, 01, 2022, 01, 2022)]
        [InlineData(2019, 01, 2022, 02, 2022)]
        [InlineData(2019, 01, 2023, 01, 2022)]
        [InlineData(2021, 11, 2021, 11, 2002)]
        [InlineData(2021, 11, 2021, 12, 2002)]
        [InlineData(2021, 11, 2022, 01, 2002)]
        [InlineData(2021, 11, 2022, 02, 2002)]
        [InlineData(2021, 11, 2022, 09, 2002)]
        [InlineData(2021, 11, 2022, 10, 2002)]
        [InlineData(2021, 11, 2022, 11, 2022)]
        [InlineData(2021, 11, 2022, 12, 2022)]
        [InlineData(2021, 11, 9999, 12, 2022)]
        [InlineData(2022, 01, 2022, 01, 2022)]
        [InlineData(2022, 01, 2022, 02, 2022)]
        [InlineData(2022, 01, 2022, 03, 2022)]
        [InlineData(2022, 01, 9999, 12, 2022)]
        [InlineData(2022, 03, 2022, 03, 2022)]
        [InlineData(2022, 03, 2022, 04, 2022)]
        [InlineData(2022, 03, 2023, 02, 2022)]
        public void ShouldWorkAsExpectedInJanuary2022(int yearOfContractStart, int monthOfContractStart, int yearOfCount, int monthOfCount, int yearOfCorrespondingList)
        {
            var priceLists = new List<PriceList>
            {
                new PriceList { StartsOn = new DateTime(2002, 01, 01) },
                new PriceList { StartsOn = new DateTime(2022, 01, 01) },
            };

            var oldContract = new Contract
            {
                TheoreticalStartOn = new DateTime(yearOfContractStart, monthOfContractStart, 01),
                CommercialOffer = new CommercialOffer { PriceLists = priceLists },
            };

            new PriceListService().GetPriceListForCount(oldContract, new DateTime(yearOfCount, monthOfCount, 01))
                .StartsOn.Year
                .Should().Be(yearOfCorrespondingList);

        }

        [Fact]
        public void ShouldUseEarliestApplicablePriceListWhenThereAreTwoChangesOnTheSameYear()
        {
            var priceLists = new List<PriceList>
            {
                new PriceList { StartsOn = new DateTime(2002, 01, 01) },
                new PriceList { StartsOn = new DateTime(2022, 01, 01) },
                new PriceList { StartsOn = new DateTime(2024, 01, 01) },
                new PriceList { StartsOn = new DateTime(2024, 05, 01) },
            };

            var contract = new Contract
            {
                TheoreticalStartOn = new DateTime(2018, 03, 01),
                CommercialOffer= new CommercialOffer { PriceLists = priceLists },
            };
            new PriceListService().GetPriceListForCount(contract, new DateTime(2021, 10, 01)).StartsOn.Year.Should().Be(2002);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2022, 01, 01)).StartsOn.Year.Should().Be(2002);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2022, 03, 01)).StartsOn.Year.Should().Be(2022);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 01, 01)).StartsOn.Year.Should().Be(2022);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 03, 01)).StartsOn.Should().Be(new DateTime(2024, 01, 01));
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 05, 01)).StartsOn.Should().Be(new DateTime(2024, 01, 01));
        }

        [Fact]
        public void ShouldUseEarliestApplicablePriceListWhenThereAreTwoChangesOnTheSameYear2()
        {
            var twoPriceListsBeforeMarch = new List<PriceList>
            {
                new PriceList { StartsOn = new DateTime(2002, 01, 01) },
                new PriceList { StartsOn = new DateTime(2022, 01, 01) },
                new PriceList { StartsOn = new DateTime(2024, 01, 01) },
                new PriceList { StartsOn = new DateTime(2024, 02, 01) },
            };

            var contract = new Contract
            {
                TheoreticalStartOn = new DateTime(2018, 03, 01),
                CommercialOffer = new CommercialOffer { PriceLists = twoPriceListsBeforeMarch },
            };

            new PriceListService().GetPriceListForCount(contract, new DateTime(2021, 10, 01)).StartsOn.Year.Should().Be(2002);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2022, 01, 01)).StartsOn.Year.Should().Be(2002);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2022, 03, 01)).StartsOn.Year.Should().Be(2022);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 01, 01)).StartsOn.Year.Should().Be(2022);
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 02, 01)).StartsOn.Year.Should().Be(2022);

            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 03, 01)).StartsOn.Should().Be(new DateTime(2024, 02, 01));
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 04, 01)).StartsOn.Should().Be(new DateTime(2024, 02, 01));
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 05, 01)).StartsOn.Should().Be(new DateTime(2024, 02, 01));
            new PriceListService().GetPriceListForCount(contract, new DateTime(2024, 12, 01)).StartsOn.Should().Be(new DateTime(2024, 02, 01));
            new PriceListService().GetPriceListForCount(contract, new DateTime(9999, 12, 01)).StartsOn.Should().Be(new DateTime(2024, 02, 01));
        }
        [Fact]
        public void ShouldGetPriceRow()
        {
            var list = new PriceList
            {
                Rows = new List<PriceRow>
                {
                    new PriceRow { MaxIncludedCount = 10},
                    new PriceRow { MaxIncludedCount = 20},
                    new PriceRow { MaxIncludedCount = 50},
                    new PriceRow { MaxIncludedCount = 100},
                },
            };
            var service = new PriceListService();
            service.GetPriceRow(list, 0).MaxIncludedCount.Should().Be(10);
            service.GetPriceRow(list, 1).MaxIncludedCount.Should().Be(10);
            service.GetPriceRow(list, 10).MaxIncludedCount.Should().Be(10);
            service.GetPriceRow(list, 11).MaxIncludedCount.Should().Be(20);
            service.GetPriceRow(list, 19).MaxIncludedCount.Should().Be(20);
            service.GetPriceRow(list, 20).MaxIncludedCount.Should().Be(20);
            service.GetPriceRow(list, 21).MaxIncludedCount.Should().Be(50);
            service.GetPriceRow(list, 50).MaxIncludedCount.Should().Be(50);
            service.GetPriceRow(list, 51).MaxIncludedCount.Should().Be(100);
            service.GetPriceRow(list, 100).MaxIncludedCount.Should().Be(100);
            Assert.Throws<InvalidOperationException>(() => service.GetPriceRow(list, 101));
        }
        [Fact]
        public void ShouldGetSinglePriceList()
        {
            var expected = new PriceList { StartsOn = new DateTime(2001) };
            var contract = ContractBuilder
                .WithPriceLists(expected)
                .WithPeriodicity(BillingPeriodicity.AnnualJanuary);

            var service = new PriceListService();
            var priceList = service.GetPriceListForCount(contract, new AccountingPeriod(2001, 01));
            priceList.Should().Be(expected);
        }

        [Fact]
        public void ShouldGetMostRecentPriceListWhenItIsInThePast()
        {
            var expected = new PriceList { StartsOn = new DateTime(2005) };
            var contract = ContractBuilder
                .WithPriceLists
                (
                    new PriceList { StartsOn = new DateTime(2003) },
                    expected,
                    new PriceList { StartsOn = new DateTime(2002) }
                )
                .WithPeriodicity(BillingPeriodicity.AnnualJanuary);

            var service = new PriceListService();
            var priceList = service.GetPriceListForCount(contract, new AccountingPeriod(2021, 01));
            priceList.Should().Be(expected);
        }
    }

    public class ContractBuilder : ContractBuilder.IPeriodicitySetter
    {

        public interface IPeriodicitySetter
        {
            Contract WithPeriodicity(BillingPeriodicity periodicity);
        }

        private Contract Contract { get; set; }


        public static IPeriodicitySetter WithPriceLists(params PriceList[] priceLists)
        {
            return new ContractBuilder
            {
                Contract = new Contract { CommercialOffer = new CommercialOffer { PriceLists = priceLists.ToList() } }
            };
        }

        public Contract WithPeriodicity(BillingPeriodicity periodicity)
        {
            Contract.BillingPeriodicity = periodicity;
            return Contract;
        }
    }
}
