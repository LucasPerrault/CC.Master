using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using Tools.Web;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/app-contacts")]
    [ApiSort(nameof(AppContact.Id))]
    public class AppContactsController
    {
        private readonly IAppContactsStore _store;

        public AppContactsController(IAppContactsStore store)
        {
            _store = store;
        }

        [HttpGet]
        public Task<Page<AppContact>> GetAsync([FromQuery]AppContactsQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
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
