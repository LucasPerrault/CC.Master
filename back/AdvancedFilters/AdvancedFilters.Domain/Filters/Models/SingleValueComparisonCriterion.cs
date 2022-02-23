using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Tools;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

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
    public class SingleEnumComparisonCriterion<T> : SingleValueComparisonCriterion<T> where T : Enum { }

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

    public class SingleFacetValueComparisonCriterion<TValue> : SingleValueComparisonCriterion<TValue>, IEnvironmentFacetCriterion
    {
        public FacetType Type { get; set; }
    }

    public class SingleFacetDateTimeValueComparisonCriterion : SingleDateTimeComparisonCriterion, IEnvironmentFacetCriterion
    {
        public FacetType Type { get; set; }
    }

    public interface IEnvironmentFacetCriterion
    {
        FacetType Type { get; set; }
        ComparisonOperators Operator { get; set; }
    }

}
