using System.Collections.Generic;

namespace AdvancedFilters.Domain.Filters.Models
{
    public class FilterCombination : IAdvancedFilter
    {
        public FilterElementTypes FilterElementType => FilterElementTypes.LogicalOperator;

        public FilterOperatorTypes Operator { get; set; }
        public IReadOnlyCollection<IAdvancedFilter> Values { get; set; }
    }

    public enum FilterOperatorTypes
    {
        And,
        Or,
    }
}
