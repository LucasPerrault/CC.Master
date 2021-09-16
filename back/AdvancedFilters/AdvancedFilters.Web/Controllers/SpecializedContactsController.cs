using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
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
    [ApiController, Route("/api/cafe/specialized-contacts")]
    [ApiSort(nameof(SpecializedContact.Id))]
    public class SpecializedContactsController
    {
        private readonly ISpecializedContactsStore _store;

        public SpecializedContactsController(ISpecializedContactsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public Task<Page<SpecializedContact>> GetAsync([FromQuery]SpecializedsContactQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
        }
    }

    public class SpecializedsContactQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<string> RoleCode { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };
        public HashSet<bool> IsConfirmed { get; set; } = new HashSet<bool> { true, false };

        public SpecializedContactFilter ToFilter()
        {
            return new SpecializedContactFilter
            {
                RoleCodes = RoleCode,
                EnvironmentIds = EnvironmentId,
                IsActive = IsActive.ToCompareBoolean(),
                IsConfirmed = IsConfirmed.ToCompareBoolean()
            };
        }
    }
}
