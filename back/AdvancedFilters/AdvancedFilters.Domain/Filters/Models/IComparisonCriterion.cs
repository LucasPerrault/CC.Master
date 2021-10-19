namespace AdvancedFilters.Domain.Filters.Models
{
    public interface IComparisonCriterion
    {
        ComparisonOperators Operator { get; }
    }

    public enum ComparisonOperators
    {
        Equals,
        NotEquals,
        GreaterThanStrictly,
        LessThanStrictly
    }
}
