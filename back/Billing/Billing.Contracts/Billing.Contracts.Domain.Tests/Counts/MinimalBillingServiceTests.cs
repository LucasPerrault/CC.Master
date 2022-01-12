using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Offers;
using Billing.Products.Domain;
using Distributors.Domain.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Billing.Contracts.Domain.Tests.Counts
{
    public class MinimalBillingServiceTests
    {

        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, true)]
        [Theory]
        public void ContractEligibility(bool isProductEligible, bool isDistributorEnforcing, bool expected)
        {
            var contract = new Contract
            {
                CommercialOffer = new CommercialOffer
                {
                    Product = new Product { IsEligibleToMinimalBilling = isProductEligible },
                },
                Distributor = new Distributor
                {
                    IsEnforcingMinimalBilling = isDistributorEnforcing,
                },
            };
            new MinimalBillingService().IsEligibleForMinimalBilling(contract)
                .Should().Be(expected);
        }

        [InlineData(0001, 12, false)]
        [InlineData(2019, 12, false)]
        [InlineData(2020, 01, true)]
        [InlineData(2020, 02, true)]
        [InlineData(2020, 03, true)]
        [InlineData(2020, 11, true)]
        [InlineData(2020, 12, true)]
        [InlineData(2021, 01, false)]
        [InlineData(9999, 12, false)]
        [Theory]
        public void EligibleContractInPeriod(int periodYear, int periodMonth, bool expected)
        {
            var contract = new Contract
            {
                MinimalBillingPercentage = 75,
                TheoreticalStartOn = new DateTime(2020, 01, 01),
                CommercialOffer = new CommercialOffer { Product = new Product { IsEligibleToMinimalBilling = true } },
                Distributor = new Distributor { IsEnforcingMinimalBilling = true },
            };

            new MinimalBillingService()
                .ShouldApplyMinimalBillingForPeriod(contract, new AccountingPeriod(periodYear, periodMonth))
                .Should().Be(expected);
        }


        [InlineData(0, 1, true)]
        [InlineData(0, 75, true)]
        [InlineData(0, 0, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(12, 0, false)]
        [Theory]
        public void EligiblePeriod_OtherFactors(int freeMonthsCount, int minimalBillingPercentage, bool expected)
        {

            var contract = new Contract
            {
                MinimalBillingPercentage = minimalBillingPercentage,
                TheoreticalFreeMonths = freeMonthsCount,
                TheoreticalStartOn = new DateTime(2020, 01, 01),
                CommercialOffer = new CommercialOffer { Product = new Product { IsEligibleToMinimalBilling = true } },
                Distributor = new Distributor { IsEnforcingMinimalBilling = true },
            };

            new MinimalBillingService()
                .ShouldApplyMinimalBillingForPeriod(contract, new AccountingPeriod(2020, 01))
                .Should().Be(expected);

        }
    }
}
