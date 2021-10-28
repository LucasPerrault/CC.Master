using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Web.Format;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/distributors")]
    [ApiSort(nameof(Distributor.Id))]
    public class DistributorsController
    {
        private readonly IDistributorsStore _store;

        public DistributorsController(IDistributorsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<Distributor>> GetAsync([FromQuery]DistributorsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        private Page<Distributor> PreparePage(Page<Distributor> src)
        {
            return new Page<Distributor>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items.WithoutLoop()
            };
        }
    }

    public class DistributorsQuery
    {
        public IPageToken Page { get; set; } = null;

        public DistributorFilter ToFilter()
        {
            return new DistributorFilter
            {

            };
        }
    }
}
