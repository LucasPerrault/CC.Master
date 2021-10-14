using System;

namespace AdvancedFilters.Domain.Filters.Models
{
    public class SingleValueComparisonCriterion<TValue> : IComparisonCriterion
        where TValue : IConvertible
    {
        public ComparisonOperators Operator { get; set; }
        public TValue Value { get; set; }
    }
}
