using System.Collections.Generic;

namespace AdvancedFilters.Domain.Facets;

public class FacetFilter
{
    public HashSet<FacetType> FacetTypes { get; set; } = new();
    public HashSet<string> ApplicationIds { get; set; } = new();
    public HashSet<string> Codes { get; set; } = new();

    private FacetFilter()
    { }

    public static FacetFilter All() => new FacetFilter();

    public static FacetFilter ForSearch(HashSet<FacetType> types, HashSet<string> codes, string applicationId) => new FacetFilter
    {
        FacetTypes = types,
        Codes = codes,
        ApplicationIds = string.IsNullOrEmpty(applicationId) ? new HashSet<string>() : new HashSet<string> { applicationId }
    };
}

public class EnvironmentFacetValueFilter
{
    public HashSet<int> EnvironmentIds { get; set; } = new();
    public HashSet<FacetIdentifier> FacetIdentifiers { get; set; } = new();
    public HashSet<FacetType> FacetTypes { get; set; } = new();
    public HashSet<string> ApplicationIds { get; set; } = new();
    public HashSet<string> Codes { get; set; } = new();

    private EnvironmentFacetValueFilter()
    { }

    public static EnvironmentFacetValueFilter All() => new EnvironmentFacetValueFilter();
    public static EnvironmentFacetValueFilter ForEnvironments(HashSet<int> envIds, HashSet<FacetIdentifier> identifiers) => new EnvironmentFacetValueFilter
    {
        EnvironmentIds = envIds,
        FacetIdentifiers = identifiers,
    };

    public static EnvironmentFacetValueFilter ForSearch(HashSet<int> envIds, HashSet<FacetType> types, HashSet<string> codes, string applicationId) => new EnvironmentFacetValueFilter
    {
        EnvironmentIds = envIds,
        FacetTypes = types,
        Codes = codes,
        ApplicationIds = string.IsNullOrEmpty(applicationId) ? new HashSet<string>() : new HashSet<string> { applicationId }
    };
}
