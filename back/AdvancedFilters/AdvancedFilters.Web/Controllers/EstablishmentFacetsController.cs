using AdvancedFilters.Domain.Facets;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers;

[ApiSort("Id")]
[ApiController, Route("/api/cafe/facets/establishments")]
public class EstablishmentFacetsController
{
    private readonly IFacetsStore _store;

    public EstablishmentFacetsController(IFacetsStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<Facet>> GetAsync([FromQuery] EstablishmentFacetQuery query)
    {
        var filter = query.ToFilter();
        return _store.GetAsync(query.Page, FacetScope.Establishment, filter);
    }

    [HttpGet("values")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<IEstablishmentFacetValue>> GetValuesAsync([FromQuery] EstablishmentFacetValueQuery query)
    {
        var filter = query.ToFilter();
        return _store.GetValuesAsync(query.Page, filter);
    }
}

public class EstablishmentFacetQuery
{
    public HashSet<string> Code { get; set; } = new();
    public string ApplicationId { get; set; }
    public HashSet<FacetType> Type { get; set; } = new();

    public IPageToken Page { get; set; }

    public FacetFilter ToFilter() => FacetFilter.ForSearch(Type, Code, ApplicationId);
}

public class EstablishmentFacetValueQuery
{
    public HashSet<int> EnvironmentId { get; set; } = new();
    public HashSet<int> EstablishmentId { get; set; } = new();
    public HashSet<string> Code { get; set; } = new();
    public string ApplicationId { get; set; }
    public HashSet<FacetType> Type { get; set; } = new();

    public IPageToken Page { get; set; }

    public EstablishmentFacetValueFilter ToFilter() => EstablishmentFacetValueFilter.ForSearch(EnvironmentId, EstablishmentId, Type, Code, ApplicationId);
}
