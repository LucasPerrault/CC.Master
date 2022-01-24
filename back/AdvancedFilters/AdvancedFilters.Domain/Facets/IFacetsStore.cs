using System.Collections.Generic;
using System.Threading.Tasks;
using Lucca.Core.Api.Abstractions.Paging;

namespace AdvancedFilters.Domain.Facets;

public class EnvironmentFacetFilter
{
    public HashSet<int> EnvironmentIds { get; set; } = new();
    public HashSet<FacetIdentifier> FacetIdentifiers { get; set; } = new();
}

public interface IFacetsStore
{
    Task<Page<IEnvironmentFacetValue>> GetAsync(IPageToken pageToken, EnvironmentFacetFilter filter);
    Task<List<IEnvironmentFacetValue>> GetAsync(EnvironmentFacetFilter filter);
}
