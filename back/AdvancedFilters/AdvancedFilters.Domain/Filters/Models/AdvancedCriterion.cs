using AdvancedFilters.Domain.Filters.Builders;

namespace AdvancedFilters.Domain.Filters.Models
{
    public abstract class AdvancedCriterion : IAdvancedFilter
    {
        public FilterElementTypes FilterElementType => FilterElementTypes.Criterion;
    }

    public abstract class AdvancedCriterion<TValue> : AdvancedCriterion
    {
        public abstract IQueryableExpressionBuilder<TValue> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory);
    }
}
