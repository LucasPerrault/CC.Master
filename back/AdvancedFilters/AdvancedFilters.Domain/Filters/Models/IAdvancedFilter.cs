namespace AdvancedFilters.Domain.Filters.Models
{
    public interface IAdvancedFilter
    {
        FilterElementTypes FilterElementType { get; }
    }

    public enum FilterElementTypes
    {
        LogicalOperator,
        Criterion,
        FacetCriterion,
    }
}
