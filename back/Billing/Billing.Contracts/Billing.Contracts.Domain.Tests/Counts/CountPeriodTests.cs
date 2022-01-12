using Billing.Contracts.Domain.Common;
using FluentAssertions;
using System;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class CountPeriodTests
    {
        [Fact]
        public void ShouldImplicitlyConvertFromDateTime()
        {
            var dateTime = new DateTime(2010, 01, 01);
            AccountingPeriod period = dateTime;
            DateTime converted = period;
            converted.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldThrowWhenConvertingFromInvalidDateTime()
        {
            var dateTime = new DateTime(2010, 01, 02);
            Assert.Throws<InvalidCountPeriodDayException>(() => ConvertFromDateTime(dateTime));
        }

        private static AccountingPeriod ConvertFromDateTime(DateTime dateTime)
        {
            return dateTime;
        }
    }
}
