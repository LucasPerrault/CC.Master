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
[ApiController, Route("/api/cafe/facets/environments")]
public class EnvironmentFacetsController
{
    private readonly IFacetsStore _store;

    public EnvironmentFacetsController(IFacetsStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<Facet>> GetAsync([FromQuery] EnvironmentFacetQuery query)
    {
        var filter = query.ToFilter();
        return _store.GetAsync(query.Page, FacetScope.Environment, filter);
    }

    [HttpGet("values")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<IEnvironmentFacetValue>> GetValuesAsync([FromQuery] EnvironmentFacetValueQuery query)
    {
        var filter = query.ToFilter();
        return _store.GetValuesAsync(query.Page, filter);
    }
}

public class EnvironmentFacetQuery
{
    public HashSet<string> Code { get; set; } = new();
    public string ApplicationId { get; set; }
    public HashSet<FacetType> Type { get; set; } = new();

    public IPageToken Page { get; set; }

    public FacetFilter ToFilter() => FacetFilter.ForSearch(Type, Code, ApplicationId);
}

public class EnvironmentFacetValueQuery
{
    public HashSet<int> EnvironmentId { get; set; } = new();
    public HashSet<string> Code { get; set; } = new();
    public string ApplicationId { get; set; }
    public HashSet<FacetType> Type { get; set; } = new();

    public IPageToken Page { get; set; }

    public EnvironmentFacetValueFilter ToFilter() => EnvironmentFacetValueFilter.ForSearch(EnvironmentId, Type, Code, ApplicationId);
}
