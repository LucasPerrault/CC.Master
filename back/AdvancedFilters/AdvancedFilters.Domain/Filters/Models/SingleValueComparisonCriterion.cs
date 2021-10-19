using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AdvancedFilters.Domain.Filters.Models
{
    public class SingleIntComparisonCriterion : SingleValueComparisonCriterion<int> { }
    public class SingleStringComparisonCriterion : SingleValueComparisonCriterion<string> { }
    public class SingleBooleanComparisonCriterion : SingleValueComparisonCriterion<bool> { }
    public class SingleDateTimeComparisonCriterion : SingleValueComparisonCriterion<DateTime>
    {
    }

    public abstract class SingleValueComparisonCriterion<TValue> : IComparisonCriterion
        where TValue : IConvertible
    {
        public ComparisonOperators Operator { get; set; }
        public TValue Value { get; set; }

        public virtual Expression<Func<TValue, bool>> Expression => Operator switch
        {
            ComparisonOperators.Equals => item => Equals(item, Value),
            ComparisonOperators.NotEquals => item => !Equals(item, Value),
            _ => throw new InvalidEnumArgumentException(nameof(Operator), (int)Operator, typeof(ComparisonOperators)),
        };
    }
}
