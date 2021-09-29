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
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools.Web;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/environments")]
    [ApiSort(nameof(Environment.Id))]
    public class EnvironmentsController
    {
        private readonly IEnvironmentsStore _store;

        public EnvironmentsController(IEnvironmentsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public Task<Page<Environment>> GetAsync([FromQuery]EnvironmentsQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
        }

        [HttpPost("search")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public Task<Page<Environment>> SearchAsync
        (
            IPageToken page,
            [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EnvironmentAdvancedCriterion>))]
            IAdvancedFilter criterion
        )
        {
            return _store.SearchAsync(page, criterion);
        }
    }

    public class EnvironmentsQuery
    {
        public IPageToken Page { get; set; } = null;

        public string Search { get; set; }
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();
        public HashSet<string> Domain { get; set; } = new HashSet<string>();
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };

        public EnvironmentFilter ToFilter()
        {
            return new EnvironmentFilter
            {
                Search = Search,
                Subdomains = Subdomain,
                Domains = Domain,
                IsActive = IsActive.ToCompareBoolean()
            };
        }
    }
}
