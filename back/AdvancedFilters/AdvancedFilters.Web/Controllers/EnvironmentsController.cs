using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using Tools.Web;

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
        public Task<Page<Environment>> GetAsync([FromQuery]EnvironmentsQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
        }
    }

    public class EnvironmentsQuery
    {
        public IPageToken Page { get; set; } = null;

        public string Search { get; set; }
        public string Subdomain { get; set; }
        public string Domain { get; set; }
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };

        public EnvironmentFilter ToFilter()
        {
            return new EnvironmentFilter
            {
                Search = Search,
                Subdomain = string.IsNullOrEmpty(Subdomain) ? CompareString.Bypass : CompareString.Equals(Subdomain),
                Domain = string.IsNullOrEmpty(Domain) ? CompareString.Bypass : CompareString.Equals(Domain),
                IsActive = IsActive.ToCompareBoolean()
            };
        }
    }
}
