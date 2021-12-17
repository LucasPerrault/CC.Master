using System;

namespace Tools
{
    public abstract class CompareDateTime
    {

        public static CompareDateTime Bypass() => new BypassCompareDateTime();
        public static CompareDateTime IsAfterOrEqual(DateTime dateTime) => new IsAfterCompareDateTime(dateTime, isStrict:false);
        public static CompareDateTime IsStrictlyAfter(DateTime dateTime) => new IsAfterCompareDateTime(dateTime, isStrict:true);
        public static CompareDateTime IsBetweenOrEqual(DateTime min, DateTime max) => new IsBetweenCompareDateTime(min, max, isStrict:false);
        public static CompareDateTime IsStrictlyBetween(DateTime min, DateTime max) => new IsBetweenCompareDateTime(min, max, isStrict:true);
        public static CompareDateTime IsBeforeOrEqual(DateTime dateTime) => new IsBeforeCompareDateTime(dateTime, isStrict:false);
        public static CompareDateTime IsStrictlyBefore(DateTime dateTime) => new IsBeforeCompareDateTime(dateTime, isStrict:true);
        public static CompareDateTime IsEqual(DateTime dateTime) => new IsEqualCompareDateTime(dateTime);
        public static CompareDateTime MatchesNone() => new NoMatchCompareDateTime();
    }

    public class BypassCompareDateTime : CompareDateTime
    {
        protected internal BypassCompareDateTime()
        { }
    }

    public class NoMatchCompareDateTime : CompareDateTime
    {
        protected internal NoMatchCompareDateTime()
        { }
    }

    public class IsEqualCompareDateTime : CompareDateTime
    {
        public DateTime Value { get; }

        protected internal IsEqualCompareDateTime(DateTime value) => Value = value;
    }

    public class IsAfterCompareDateTime : CompareDateTime
    {
        public DateTime Value { get; }
        public bool IsStrict { get; }

        protected internal IsAfterCompareDateTime(DateTime value, bool isStrict)
        {
            Value = value;
            IsStrict = isStrict;
        }
    }

    public class IsBeforeCompareDateTime : CompareDateTime
    {
        public DateTime Value { get; }
        public bool IsStrict { get; }

        protected internal IsBeforeCompareDateTime(DateTime value, bool isStrict)
        {
            Value = value;
            IsStrict = isStrict;
        }
    }

    public class IsBetweenCompareDateTime : CompareDateTime
    {
        public DateTime Min { get; }
        public DateTime Max { get; }
        public bool IsStrict { get; }

        protected internal IsBetweenCompareDateTime(DateTime min, DateTime max, bool isStrict)
        {
            Min = min;
            Max = max;
            IsStrict = isStrict;
        }
    }




    public abstract class CompareNullableDateTime
    {
        public static CompareNullableDateTime Bypass() => CompareDateTime.Bypass().OrNull();
        public static CompareNullableDateTime IsNull() => new IsNullCompareNullableDateTime();
        public static CompareNullableDateTime AnyNotNull() => CompareDateTime.Bypass().AndNotNull();
        public static CompareNullableDateTime MatchesNone() => CompareDateTime.MatchesNone().AndNotNull();
    }

    public class IsNullCompareNullableDateTime : CompareNullableDateTime
    {
        protected internal IsNullCompareNullableDateTime()
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
