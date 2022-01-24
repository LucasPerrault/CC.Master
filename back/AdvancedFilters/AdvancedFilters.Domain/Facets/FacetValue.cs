namespace AdvancedFilters.Domain.Facets;

public interface IFacetValue
{
    public int FacetId { get; }
    public int EnvironmentId { get; }
}

public interface IEnvironmentFacetValue : IFacetValue
{
    public FacetType Type { get; }
}

public class EnvironmentFacetValue<T> : IEnvironmentFacetValue
{
    public T Value { get; set; }
    public int FacetId { get; set; }
    public int EnvironmentId { get; set; }
    public FacetType Type { get; set; }
}
