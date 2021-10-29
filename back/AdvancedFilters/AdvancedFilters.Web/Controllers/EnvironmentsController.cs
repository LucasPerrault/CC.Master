using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services;
using AdvancedFilters.Web.Binding;
using AdvancedFilters.Web.Format;
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
        private readonly IExportService _exportService;

        public EnvironmentsController(IEnvironmentsStore store, IExportService exportService)
        {
            _store = store;
            _exportService = exportService;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<Environment>> GetAsync([FromQuery]EnvironmentsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        [HttpGet("clusters")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public Task<List<string>> GetAsync()
        {
            return _store.GetClustersAsync();
        }

        [HttpPost("search")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<Environment>> SearchAsync
        (
            IPageToken pageToken,
            [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EnvironmentAdvancedCriterion>))]
            IAdvancedFilter criterion
        )
        {
            var page = await _store.SearchAsync(pageToken, criterion);
            return PreparePage(page);
        }

        [HttpPost("export")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<FileStreamResult> ExportAsync
        (
            [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<EnvironmentAdvancedCriterion>))]
            IAdvancedFilter criterion
        )
        {
            var environments = await _store.SearchAsync(criterion);

            var filename = $"export-{System.DateTime.Now:yyyyMMdd-HHmmss}.csv";
            return _exportService.Export(environments, filename);

        }

        private Page<Environment> PreparePage(Page<Environment> src)
        {
            return new Page<Environment>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items.WithoutLoop()
            };
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
