using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedFilters.Application;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services;
using AdvancedFilters.Web.Binding;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;

namespace AdvancedFilters.Web.Controllers;

[ApiController, Route("/api/cafe/establishments")]
[ApiSort(nameof(Establishment.Id))]
public class EstablishmentsController
{
    private readonly IEstablishmentsStore _store;
    private readonly IEstablishmentPopulator _populator;
    private readonly IExportService _exportService;

    public EstablishmentsController(IEstablishmentsStore store, IExportService exportService, IEstablishmentPopulator populator)
    {
        _store = store;
        _exportService = exportService;
        _populator = populator;
    }

    [HttpGet]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<Establishment>> GetAsync([FromQuery]EstablishmentsQuery query)
    {
        return _store.GetAsync(query.Page, query.ToFilter());
    }

    [HttpPost("search")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public async Task<Page<Establishment>> SearchAsync
    (
        IPageToken pageToken,
        [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EstablishmentSearchBody, EstablishmentAdvancedCriterion>))]
        EstablishmentSearchBody body
    )
    {
        var page = await _store.SearchAsync(pageToken, body.Criterion);
        await _populator.PopulateAsync(page.Items, body.FacetIds);
        return page;
    }

    [HttpPost("export")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public async Task<FileStreamResult> ExportAsync
    (
        [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EstablishmentAdvancedCriterion>))]
        IAdvancedFilter criterion
    )
    {
        var establishments = await _store.SearchAsync(criterion);

        var filename = $"export-{System.DateTime.Now:yyyyMMdd-HHmmss}.csv";
        return _exportService.Export(establishments, filename);

    }

    public class EstablishmentSearchBody
    {
        public HashSet<int> FacetIds { get; set; } = new();
        public IAdvancedFilter Criterion { get; set; }
    }

    public class EstablishmentsQuery
    {
        public IPageToken Page { get; set; } = null;

        public EstablishmentFilter ToFilter()
        {
            return new EstablishmentFilter();
        }
    }
}
