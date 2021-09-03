using System;

namespace Tools
{
    public abstract class CompareDateTime
    {
        public static CompareDateTime Bypass() => new BypassCompareDateTime();
        public static CompareDateTime IsAfterOrEqual(DateTime dateTime) => new IsAfterOrEqualCompareDateTime(dateTime);
        public static CompareDateTime IsStrictlyAfter(DateTime dateTime) => new IsStrictlyAfterCompareDateTime(dateTime);
    }

    public class BypassCompareDateTime : CompareDateTime
    {

        protected internal BypassCompareDateTime()
        { }

    }

    public class IsAfterOrEqualCompareDateTime : CompareDateTime
    {
        public DateTime Value { get; }

        protected internal IsAfterOrEqualCompareDateTime(DateTime value)
        {
            Value = value;
        }
    }

    public class IsStrictlyAfterCompareDateTime : CompareDateTime
    {
        public DateTime Value { get; }

        protected internal IsStrictlyAfterCompareDateTime(DateTime value)
        {
            Value = value;
        }
    }




    public abstract class CompareNullableDateTime
    {
        public static CompareNullableDateTime Bypass() => CompareDateTime.Bypass().OrNull();
        public static CompareNullableDateTime IsNull() => new IsNullCompareNullableDateTimeQueryBuilder();
        public static CompareNullableDateTime AnyNotNull() => CompareDateTime.Bypass().AndNotNull();
    }

    public class IsNullCompareNullableDateTimeQueryBuilder : CompareNullableDateTime
    {
        protected internal IsNullCompareNullableDateTimeQueryBuilder()
        { }
    }

    public class OrNullCompareNullableDatetime : CompareNullableDateTime
    {
        public CompareDateTime CompareDateTime { get; }
        protected internal OrNullCompareNullableDatetime(CompareDateTime compareDateTime)
        {
            CompareDateTime = compareDateTime;
        }
    }

    public class AndNotNullCompareNullableDateTime : CompareNullableDateTime
    {
        public CompareDateTime CompareDateTime { get; }

        protected internal AndNotNullCompareNullableDateTime(CompareDateTime compareDateTime)
        {
            CompareDateTime = compareDateTime;
        }
    }

    public static class CompareNullableDateTimeExtensions
    {

        public static CompareNullableDateTime OrNull(this CompareDateTime compareDateTime) => new OrNullCompareNullableDatetime(compareDateTime);
        public static CompareNullableDateTime AndNotNull(this CompareDateTime compareDateTime) => new AndNotNullCompareNullableDateTime(compareDateTime);
    }
}
