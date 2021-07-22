using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/clients")]
    public class ClientsController
    {
        private readonly ClientsRepository _clientsRepository;

        public ClientsController(ClientsRepository clientsRepository)
        {
            _clientsRepository = clientsRepository ?? throw new ArgumentNullException(nameof(clientsRepository));
        }

        [HttpPut("{id}")]
        public Task PutAsync([FromRoute]Guid id, [FromBody]Client client, [FromQuery]string subdomain)
        {
            if (string.IsNullOrEmpty(subdomain))
            {
                throw new BadRequestException($"{nameof(subdomain)} parameter is mandatory");
            }

            return _clientsRepository.PutAsync(id, client, subdomain);
        }
    }
}
