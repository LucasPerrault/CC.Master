using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
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
        public Task<Page<ClientContact>> GetAsync([FromQuery]ClientContactsQuery query)
        {
            return _store.GetAsync(query.Page, query.ToFilter());
        }
    }

    public class ClientContactsQuery
    {
        public IPageToken Page { get; set; } = null;

        public IReadOnlyCollection<int> RoleId { get; set; }
        public IReadOnlyCollection<Guid> ClientId { get; set; }
        public IReadOnlyCollection<int> EnvironmentId { get; set; }
        public IReadOnlyCollection<int> EstablishmentId { get; set; }
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true, false };
        public HashSet<bool> IsConfirmed { get; set; } = new HashSet<bool> { true, false };

        public ClientContactFilter ToFilter()
        {
            return new ClientContactFilter
            {
                RoleIds = RoleId,
                ClientIds = ClientId,
                EnvironmentIds = EnvironmentId,
                EstablishmentIds = EstablishmentId,
                IsActive = IsActive.ToCompareBoolean(),
                IsConfirmed = IsConfirmed.ToCompareBoolean()
            };
        }
    }
}
