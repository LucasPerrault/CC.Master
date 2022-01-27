using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public abstract class CompareString
    {

        protected internal CompareString()
        { }

        public static CompareString Bypass => new BypassCompareString();
        public static CompareString Equals(HashSet<string> values) => new EqualsCompareString(values);
        public static CompareString Equals(params string[] values) => Equals(values.ToHashSet());
        public static CompareString DoesNotEqual(HashSet<string> values) => new DoesNotEqualCompareString(values.ToHashSet());
        public static CompareString DoesNotEqual(params string[] values) => DoesNotEqual(values.ToHashSet());
        public static CompareString StartsWith(string value) => new StartsWithCompareString(value);
    }

    public class BypassCompareString : CompareString
    {
        protected internal BypassCompareString()
        { }
    }

    public class EqualsCompareString : CompareString
    {
        public HashSet<string> Values { get; }

        protected internal EqualsCompareString(HashSet<string> values)
        {
            Values = values;
        }
    }

    public class DoesNotEqualCompareString : CompareString
    {
        public HashSet<string> Values { get; }

        protected internal DoesNotEqualCompareString(HashSet<string> values)
        {
            Values = values;
        }
    }

    public class StartsWithCompareString : CompareString
    {
        public string Value { get; }

        protected internal StartsWithCompareString(string value)
        {
            Value = value;
        }
    }
}
