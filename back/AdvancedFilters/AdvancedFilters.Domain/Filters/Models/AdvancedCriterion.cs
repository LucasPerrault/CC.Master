namespace AdvancedFilters.Domain.Filters.Models
{
    public abstract class AdvancedCriterion : IAdvancedFilter
    {
        public FilterElementTypes FilterElementType => FilterElementTypes.Criterion;
    }
}
