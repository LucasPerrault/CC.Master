using AdvancedFilters.Domain.Filters.Models;
using System;

namespace AdvancedFilters.Infra.Filters.Builders.Exceptions
{
    public class MissingItemsMatchedFieldException<TItem> : ApplicationException
    {
        public MissingItemsMatchedFieldException()
            : base($"{typeof(TItem).Name} filter is missing the {nameof(IListCriterion.ItemsMatched)} field")
        { }
    }
}
