
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Facets;

public interface IFacetsStore
{
    Task<Page<Facet>> GetAsync(IPageToken pageToken, FacetScope scope, FacetFilter filter);
    Task<Page<IEnvironmentFacetValue>> GetValuesAsync(IPageToken pageToken, EnvironmentFacetValueFilter filter);
    Task<Page<IEstablishmentFacetValue>> GetValuesAsync(IPageToken pageToken, EstablishmentFacetValueFilter filter);
    Task<List<IEnvironmentFacetValue>> GetValuesAsync(EnvironmentFacetValueFilter filter);
    Task<List<IEstablishmentFacetValue>> GetValuesAsync(EstablishmentFacetValueFilter filter);
    Expression<Func<Environment, bool>> GetEnvFacetFilter(EnvironmentFacetsAdvancedCriterion criterion);
    Expression<Func<Establishment, bool>> GetEstablishmentFacetFilter(EstablishmentFacetsAdvancedCriterion criterion);
}
