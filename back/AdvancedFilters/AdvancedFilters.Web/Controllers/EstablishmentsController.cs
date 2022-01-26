using System.Threading.Tasks;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
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

    public EstablishmentsController(IEstablishmentsStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<Establishment>> GetAsync([FromQuery]EstablishmentsQuery query)
    {
        return _store.GetAsync(query.Page, query.ToFilter());
    }

    [HttpPost("search")]
    [ForbidIfMissing(Operation.ReadAllCafe)]
    public Task<Page<Establishment>> SearchAsync
    (
        IPageToken pageToken,
        [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EstablishmentAdvancedCriterion>))]
        IAdvancedFilter criterion
    )
    {
        return _store.SearchAsync(pageToken, criterion);
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
