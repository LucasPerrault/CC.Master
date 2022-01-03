using AdvancedFilters.Domain.Billing.Filters;
using AdvancedFilters.Domain.Billing.Interfaces;
using AdvancedFilters.Domain.Billing.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/clients")]
    [ApiSort(nameof(Client.Id))]
    public class ClientsController
    {
        private readonly IClientsStore _store;

        public ClientsController(IClientsStore store)
        {
            _store = store;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadAllCafe)]
        public async Task<Page<Client>> GetAsync([FromQuery] ClientsQuery query)
        {
            var page = await _store.GetAsync(query.Page, query.ToFilter());
            return PreparePage(page);
        }

        private Page<Client> PreparePage(Page<Client> src)
        {
            return new Page<Client>
            {
                Count = src.Count,
                Prev = src.Prev,
                Next = src.Next,
                Items = src.Items
            };
        }
    }

    public class ClientsQuery
    {
        public IPageToken Page { get; set; } = null;

        public string Search { get; set; }

        public ClientFilter ToFilter()
        {
            return new ClientFilter
            {
                Search = Search,
            };
        }
    }
}
