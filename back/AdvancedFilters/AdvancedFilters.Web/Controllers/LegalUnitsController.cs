using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
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
        public Task<Page<LegalUnit>> GetAsync([FromQuery]LegalUnitsQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
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
