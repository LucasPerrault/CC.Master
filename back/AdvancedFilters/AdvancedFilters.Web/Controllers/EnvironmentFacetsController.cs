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

    [HttpGet("values")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<IEnvironmentFacetValue>> GetAsync([FromQuery]EnvironmentFacetQuery query)
    {
        var filter = query.ToFilter();
        return _store.GetAsync(query.Page, filter);
    }
}

public class EnvironmentFacetQuery
{

    public HashSet<int> EnvironmentId { get; set; } = new();
    public HashSet<string> Code { get; set; } = new();
    public string ApplicationId { get; set; }
    public HashSet<FacetType> Type { get; set; } = new();

    public IPageToken Page { get; set; }

    public EnvironmentFacetFilter ToFilter() => EnvironmentFacetFilter.ForSearch(EnvironmentId, Type, Code, ApplicationId);
}
