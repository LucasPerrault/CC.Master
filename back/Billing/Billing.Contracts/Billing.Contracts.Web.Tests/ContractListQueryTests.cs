using FluentAssertions;
using System;
using Tools;
using Xunit;

namespace Billing.Contracts.Web.Tests
{
    public class ContractListQueryTests
    {
        [Fact]
        public void StarsOnConversionTests()
        {
            new ContractListQuery()
                .ToFilter()
                .StartsOn
                .Should().BeOfType<BypassCompareDateTime>();

            new ContractListQuery
                {
                    TheoreticalStartOn = new DateTime(2021, 01, 01)
                }.ToFilter()
                .StartsOn
                .Should().BeOfType<IsEqualCompareDateTime>().Which.Value.Should().Be(new DateTime(2021, 01, 01));

            var startedOn = new ContractListQuery
                {
                    WasStartedOn = new DateTime(2021, 01, 01)
                }.ToFilter()
                .StartsOn
                .Should().BeOfType<IsBeforeCompareDateTime>();
            startedOn.Which.IsStrict.Should().BeFalse();
            startedOn.Which.Value.Should().Be(new DateTime(2021, 01, 01));

            var startedOnAndTheoreticalBefore = new ContractListQuery
                {
                    WasStartedOn = new DateTime(2021, 01, 01),
                    TheoreticalStartOn = new DateTime(2020, 12, 31),
                }.ToFilter()
                .StartsOn
                .Should().BeOfType<IsBeforeCompareDateTime>();
            startedOnAndTheoreticalBefore.Which.IsStrict.Should().BeFalse();
            startedOnAndTheoreticalBefore.Which.Value.Should().Be(new DateTime(2021, 01, 01));

            new ContractListQuery
                {
                    WasStartedOn = new DateTime(2021, 01, 01),
                    TheoreticalStartOn = new DateTime(2021, 01, 02),
                }.ToFilter()
                .StartsOn
                .Should().BeOfType<NoMatchCompareDateTime>();
        }
        [Fact]
        public void EndsOnConversionTests()
        {
            new ContractListQuery()
                .ToFilter()
                .EndsOn
                .Should().BeOfType<OrNullCompareNullableDatetime>()
                .Which.CompareDateTime.Should().BeOfType<BypassCompareDateTime>();

            new ContractListQuery
                {
                    TheoreticalEndOn = new DateTime(2021, 01, 01)
                }.ToFilter()
                .EndsOn
                .Should().BeOfType<AndNotNullCompareNullableDateTime>()
                .Which.CompareDateTime.Should().BeOfType<IsEqualCompareDateTime>()
                .Which.Value.Should().Be(new DateTime(2021, 01, 01));

            var notEndedOn = new ContractListQuery
                {
                    WasNotEndedOn = new DateTime(2021, 01, 01)
                }.ToFilter()
                .EndsOn
                .Should().BeOfType<OrNullCompareNullableDatetime>()
                .Which.CompareDateTime.Should().BeOfType<IsBeforeCompareDateTime>();
            notEndedOn.Which.IsStrict.Should().BeFalse();
            notEndedOn.Which.Value.Should().Be(new DateTime(2021, 01, 01));

        new ContractListQuery
            {
                WasNotEndedOn = new DateTime(2021, 01, 01),
                TheoreticalEndOn = new DateTime(2021, 01, 02),
            }.ToFilter()
            .EndsOn
            .Should().BeOfType<AndNotNullCompareNullableDateTime>()
            .Which.CompareDateTime.Should().BeOfType<IsEqualCompareDateTime>()
            .Which.Value.Should().Be(new DateTime(2021, 01, 02));

        new ContractListQuery
            {
                WasNotEndedOn = new DateTime(2021, 01, 01),
                TheoreticalEndOn = new DateTime(2020, 12, 31),
            }.ToFilter()
            .EndsOn
            .Should().BeOfType<AndNotNullCompareNullableDateTime>()
            .Which.CompareDateTime.Should().BeOfType<NoMatchCompareDateTime>();
        }
    }
}
