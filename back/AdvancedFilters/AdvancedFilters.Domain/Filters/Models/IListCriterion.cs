namespace AdvancedFilters.Domain.Filters.Models
{
    public interface IListCriterion
    {
        public ItemsMatching ItemsMatched { get; set; }
    }

    public enum ItemsMatching
    {
        Any = 1,
        All = 2
    }
}
