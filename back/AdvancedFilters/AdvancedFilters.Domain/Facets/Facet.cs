namespace AdvancedFilters.Domain.Facets;

public enum FacetType
{
    Unknown = 0,
    Integer = 1,
    DateTime = 2,
    Decimal = 3,
    Percentage = 4,
    String = 5,
}

public enum FacetScope
{
    Unknown = 0,
    Environment = 1,
    Establishment = 2,
}

public class FacetIdentifier
{
    public string Code { get; set; }
    public string ApplicationId { get; set; }
}

public class Facet : FacetIdentifier
{
    public int Id { get; set; }
    public FacetType Type { get; set; }
    public FacetScope Scope { get; set; }
}
