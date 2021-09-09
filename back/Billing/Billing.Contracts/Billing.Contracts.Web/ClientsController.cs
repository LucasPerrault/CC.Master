using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/clients")]
    [ApiSort(nameof(Client.Id))]
    public class ClientsController
    {
        private readonly ClientsRepository _clientsRepository;

        public ClientsController(ClientsRepository clientsRepository)
        {
            _clientsRepository = clientsRepository ?? throw new ArgumentNullException(nameof(clientsRepository));
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<Page<Client>> GetAsync(IPageToken pageToken, [FromQuery] ClientListQuery query)
        {
            return _clientsRepository.GetPageAsync(pageToken, query.ToFilter());
        }

        [HttpPut("{id:guid}")]
        public Task PutAsync([FromRoute]Guid id, [FromBody]Client client, [FromQuery]string subdomain)
        {
            if (string.IsNullOrEmpty(subdomain))
            {
                throw new BadRequestException($"{nameof(subdomain)} parameter is mandatory");
            }

            return _clientsRepository.PutAsync(id, client, subdomain);
        }
    }

    public class ClientListQuery
    {
        public string Search { get; set; }

        public ClientFilter ToFilter() => new ClientFilter
        {
            Search = Search?.Split(" ").ToHashSet() ?? null
        };
    }
}
