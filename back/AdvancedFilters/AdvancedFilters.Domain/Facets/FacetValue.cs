namespace AdvancedFilters.Domain.Facets;

public interface IFacetValue
{
    public int FacetId { get; }
    public int EnvironmentId { get; }
    public FacetIdentifier Facet { get; set; }
}

public interface IEnvironmentFacetValue : IFacetValue
{
    public int Id { get; }
    public FacetType Type { get; }
}

public class EnvironmentFacetValue<T> : IEnvironmentFacetValue
{
    public int Id { get; set; }
    public T Value { get; set; }
    public int FacetId { get; set; }
    public int EnvironmentId { get; set; }
    public FacetIdentifier Facet { get; set; }
    public FacetType Type { get; set; }
}
