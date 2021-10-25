using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Web.Format;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/legal-units")]
    [ApiSort(nameof(LegalUnit.Id))]
    public class LegalUnitsController
    {
        private readonly ILegalUnitsStore _store;

        public LegalUnitsController(ILegalUnitsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<LegalUnit>> GetAsync([FromQuery]LegalUnitsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        [HttpGet("countries")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public Task<Page<Country>> GetCountriesAsync()
        {
            return _store.GetAllCountriesAsync();
        }

        private Page<LegalUnit> PreparePage(Page<LegalUnit> src)
        {
            return new Page<LegalUnit>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items.WithoutLoop()
            };
        }
    }

    public class LegalUnitsQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<int> CountryId { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }

        public LegalUnitFilter ToFilter()
        {
            return new LegalUnitFilter
            {
                CountryIds = CountryId,
                EnvironmentIds = EnvironmentId
            };
        }
    }
}
