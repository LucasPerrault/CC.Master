using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Contracts;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/contracts")]
    public class ContractsController
    {
        private readonly ContractsRepository _contractsRepository;

        public ContractsController(ContractsRepository contractsRepository)
        {
            _contractsRepository = contractsRepository;
        }

        [HttpGet]
        public async Task<Page<Contract>> GetAsync()
        {
            var contracts = await _contractsRepository.GetAsync();

            return new Page<Contract>
            {
                Count = contracts.Count,
                Items = contracts
            };
        }
    }
}
