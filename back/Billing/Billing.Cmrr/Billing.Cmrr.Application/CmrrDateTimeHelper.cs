using System;

namespace Billing.Cmrr.Application
{
    public static class CmrrDateTimeHelper
    {
        private static bool IsFirstDayOfMonth(DateTime date) => date.Day == 1;

        public static void ThrowIfDatesAreNotAtFirstDayOfMonth(params DateTime[] dates)
        {
            foreach (var date in dates)
            {
                if (!IsFirstDayOfMonth(date))
                    throw new ArgumentException($"{nameof(date)} must be on the first day of month");
            }
        }

        public static int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }
    }
}
