using System.Collections.Generic;

namespace AdvancedFilters.Domain.Facets;

public class EnvironmentFacetFilter
{
    public HashSet<int> EnvironmentIds { get; set; } = new();
    public HashSet<FacetIdentifier> FacetIdentifiers { get; set; } = new();
    public HashSet<FacetType> FacetTypes { get; set; } = new();
    public HashSet<string> ApplicationIds { get; set; } = new();
    public HashSet<string> Codes { get; set; } = new();

    private EnvironmentFacetFilter()
    { }

    public static EnvironmentFacetFilter All() => new EnvironmentFacetFilter();
    public static EnvironmentFacetFilter ForEnvironments(HashSet<int> envIds, HashSet<FacetIdentifier> identifiers) => new EnvironmentFacetFilter
    {
        EnvironmentIds = envIds,
        FacetIdentifiers = identifiers,
    };

    public static EnvironmentFacetFilter ForSearch(HashSet<int> envIds, HashSet<FacetType> types, HashSet<string> codes, string applicationId) => new EnvironmentFacetFilter
    {
        EnvironmentIds = envIds,
        FacetTypes = types,
        Codes = codes,
        ApplicationIds = string.IsNullOrEmpty(applicationId) ? new HashSet<string>() : new HashSet<string> { applicationId }
    };
}
