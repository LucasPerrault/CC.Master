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

public interface IEstablishmentFacetValue : IFacetValue
{
    public int Id { get; }
    public FacetType Type { get; }
    public int EstablishmentId { get; }
}

public class EstablishmentFacetValue<T> : IEstablishmentFacetValue
{
    public int Id { get; set; }
    public T Value { get; set; }
    public int FacetId { get; set; }
    public int EnvironmentId { get; set; }
    public int EstablishmentId { get; set; }
    public FacetIdentifier Facet { get; set; }
    public FacetType Type { get; set; }
}
