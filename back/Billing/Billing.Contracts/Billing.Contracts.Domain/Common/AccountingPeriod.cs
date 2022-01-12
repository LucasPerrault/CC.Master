using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Common
{


    public class AccountingPeriod : ValueObject, IComparable
    {
        public int Year { get; }
        public int Month { get; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Year;
                yield return Month;
            }

        }

        public AccountingPeriod(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public static implicit operator DateTime(AccountingPeriod p) => new DateTime(p.Year, p.Month, 1);
        public static implicit operator AccountingPeriod(DateTime d) => d.Day == 1
            ? new AccountingPeriod(d.Year, d.Month)
            : throw new InvalidCountPeriodDayException();

        public int CompareTo(object? obj)
        {
            if (obj is not AccountingPeriod otherPeriod)
            {
                return 1;
            }

            return TotalMonths() - otherPeriod.TotalMonths();
        }

        private int TotalMonths() => Year * 12 + Month;

        public AccountingPeriod AddMonth(int monthCount)
        {
            return ((DateTime)this).AddMonths(monthCount);
        }
    }

    public class InvalidCountPeriodDayException : DomainException
    {
        public InvalidCountPeriodDayException() : base(DomainExceptionCode.BadRequest, "When expressed as a date, accounting period should be the first of the month")
        { }
    }
}
