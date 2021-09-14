using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/app-instances")]
    [ApiSort(nameof(AppInstance.Id))]
    public class AppInstancesController
    {
        private readonly IAppInstancesStore _store;

        public AppInstancesController(IAppInstancesStore store)
        {
            _store = store;
        }

        [HttpGet]
        public Task<Page<AppInstance>> GetAsync([FromQuery]AppInstancesQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
        }
    }

    public class AppInstancesQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<int> Id { get; set; }
        public IReadOnlyCollection<string> ApplicationId { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }

        public AppInstanceFilter ToFilter()
        {
            return new AppInstanceFilter
            {
                Ids = Id,
                ApplicationIds = ApplicationId,
                EnvironmentIds = EnvironmentId
            };
        }
    }
}
