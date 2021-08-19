using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Contracts;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/contracts")]
    [ApiSort(nameof(Contract.Id))]
    public class ContractsController
    {
        private readonly ContractsRepository _contractsRepository;

        public ContractsController(ContractsRepository contractsRepository)
        {
            _contractsRepository = contractsRepository;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<Page<Contract>> GetAsync(IPageToken pageToken)
        {
            return _contractsRepository.GetPageAsync(pageToken);
        }
    }
}
