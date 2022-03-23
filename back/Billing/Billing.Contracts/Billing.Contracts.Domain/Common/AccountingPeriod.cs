using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using NExtends.Primitives.DateTimes;
using Tools;

namespace Billing.Contracts.Domain.Common
{


    public class AccountingPeriod : ValueObject, IComparable
    {
        public int Year { get; init; }
        public int Month { get; init; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Year;
                yield return Month;
            }

        }

        private const string DateFormat = "yyyy-MM";
        private static readonly IFormatProvider FormatProvider = new AccountingPeriodFormatProvider();

        private AccountingPeriod() { }

        public static implicit operator DateTime(AccountingPeriod p) => new DateTime(p.Year, p.Month, 1);
        public static implicit operator AccountingPeriod(DateTime d) => d.Day == 1
            ? new AccountingPeriod { Year = d.Year, Month = d.Month }
            : throw new InvalidCountPeriodDayException();

        public DateTime LastOfMonth() => ( (DateTime) this ).LastOfMonth();

        public int CompareTo(object? obj)
        {
            if (obj is not AccountingPeriod otherPeriod)
            {
                return 1;
            }

            return TotalMonths() - otherPeriod.TotalMonths();
        }

        public override string ToString() => ((DateTime)this).ToString(DateFormat, CultureInfo.InvariantCulture);

        public static AccountingPeriod Parse(string period) => DateTime.Parse(period, FormatProvider);
        private int TotalMonths() => Year * 12 + Month;

        private class AccountingPeriodFormatProvider : IFormatProvider
        {
            public object? GetFormat(Type? formatType) => DateFormat;
        }
    }

    public class InvalidCountPeriodDayException : DomainException
    {
        public InvalidCountPeriodDayException() : base(DomainExceptionCode.BadRequest, "When expressed as a date, accounting period should be the first of the month")
        { }
    }
}
