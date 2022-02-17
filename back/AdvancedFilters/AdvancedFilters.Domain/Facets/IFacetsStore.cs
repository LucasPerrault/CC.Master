using System.Collections.Generic;
using System.Threading.Tasks;
using Lucca.Core.Api.Abstractions.Paging;

namespace AdvancedFilters.Domain.Facets;

public interface IFacetsStore
{
    Task<Page<Facet>> GetAsync(IPageToken pageToken, FacetScope scope, FacetFilter filter);
    Task<Page<IEnvironmentFacetValue>> GetValuesAsync(IPageToken pageToken, EnvironmentFacetValueFilter filter);
    Task<Page<IEstablishmentFacetValue>> GetValuesAsync(IPageToken pageToken, EstablishmentFacetValueFilter filter);
    Task<List<IEnvironmentFacetValue>> GetValuesAsync(EnvironmentFacetValueFilter filter);
    Task<List<IEstablishmentFacetValue>> GetValuesAsync(EstablishmentFacetValueFilter filter);
    Task<IReadOnlyCollection<Facet>> GetByIdentifiersAsync(HashSet<FacetIdentifier> identifiers);
}
