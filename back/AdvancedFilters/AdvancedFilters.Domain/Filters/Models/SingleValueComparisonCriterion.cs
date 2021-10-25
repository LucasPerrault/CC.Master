using Storage.Infra.Extensions;
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
        public override Expression<Func<DateTime, bool>> Expression => Operator switch
        {
            ComparisonOperators.Equals => item => item.Year == Value.Year && item.Month == Value.Month && item.Day == Value.Day,
            ComparisonOperators.StrictlyGreaterThan => item => item > Value,
            ComparisonOperators.StrictlyLessThan => item => item < Value,
            _ => base.Expression
        };
    }
    public class SingleGuidComparisonCriterion : SingleValueComparisonCriterion<Guid> { }

    public abstract class SingleValueComparisonCriterion<TValue> : IComparisonCriterion
    {
        public ComparisonOperators Operator { get; set; }
        public TValue Value { get; set; }

        public virtual Expression<Func<TValue, bool>> Expression => GetExpression(Operator);

        protected Expression<Func<TValue, bool>> GetExpression(ComparisonOperators op) => op switch
        {
            ComparisonOperators.Equals => item => Equals(item, Value),
            ComparisonOperators.NotEquals => GetExpression(ComparisonOperators.Equals).Inverse(),
            _ => throw new InvalidEnumArgumentException(nameof(Operator), (int) Operator, typeof(ComparisonOperators)),
        };
}
}
