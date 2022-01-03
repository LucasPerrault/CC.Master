using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Services;
using AdvancedFilters.Web.Binding;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools.Web;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/app-contacts")]
    [ApiSort(nameof(AppContact.Id))]
    public class AppContactsController
    {
        private readonly IAppContactsStore _store;
        private readonly IExportService _exportService;

        public AppContactsController(IAppContactsStore store, IExportService exportService)
        {
            _store = store;
            _exportService = exportService;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<AppContact>> GetAsync([FromQuery]AppContactsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        [HttpPost("search")]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<AppContact>> SearchAsync
        (
            IPageToken pageToken,
            [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<AppContactAdvancedCriterion>))]
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
            [FromBody, ModelBinder(BinderType = typeof(AdvancedFilterModelBinder<AppContactAdvancedCriterion>))]
            IAdvancedFilter criterion
        )
        {
            var contacts = await _store.SearchAsync(criterion);
            var filename = $"export-{System.DateTime.Now:yyyyMMdd-HHmmss}.csv";
            return _exportService.Export(contacts, filename);
        }

        private Page<AppContact> PreparePage(Page<AppContact> src)
        {
            return new Page<AppContact>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items
            };
        }
    }

    public class AppContactsQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<string> ApplicationId { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };
        public HashSet<bool> IsConfirmed { get; set; } = new HashSet<bool> { true, false };

        public AppContactFilter ToFilter()
        {
            return new AppContactFilter
            {
                ApplicationIds = ApplicationId,
                EnvironmentIds = EnvironmentId,
                IsActive = IsActive.ToCompareBoolean(),
                IsConfirmed = IsConfirmed.ToCompareBoolean()
            };
        }
    }
}
