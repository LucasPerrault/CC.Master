using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Web.Format;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools.Web;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/client-contacts")]
    [ApiSort(nameof(ClientContact.Id))]
    public class ClientContactsController
    {
        private readonly IClientContactsStore _store;

        public ClientContactsController(IClientContactsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<ClientContact>> GetAsync([FromQuery]ClientContactsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        private Page<ClientContact> PreparePage(Page<ClientContact> src)
        {
            return new Page<ClientContact>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items.WithoutLoop()
            };
        }
    }

    public class ClientContactsQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<string> RoleCode { get; set; }
        public IReadOnlyCollection<Guid> ClientId { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }
        public IReadOnlyCollection<int> EstablishmentId { get; set; }
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };
        public HashSet<bool> IsConfirmed { get; set; } = new HashSet<bool> { true, false };

        public ClientContactFilter ToFilter()
        {
            return new ClientContactFilter
            {
                RoleCodes = RoleCode,
                ClientIds = ClientId,
                EnvironmentIds = EnvironmentId,
                EstablishmentIds = EstablishmentId,
                IsActive = IsActive.ToCompareBoolean(),
                IsConfirmed = IsConfirmed.ToCompareBoolean()
            };
        }
    }
}
