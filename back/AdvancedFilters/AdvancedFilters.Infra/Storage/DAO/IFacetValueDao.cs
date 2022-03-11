namespace AdvancedFilters.Domain.Facets.DAO;

public interface IFacetValueDao
{
    public int Id { get; set; }
    public int FacetId { get; set; }
    public Facet Facet { get; set; }
}
