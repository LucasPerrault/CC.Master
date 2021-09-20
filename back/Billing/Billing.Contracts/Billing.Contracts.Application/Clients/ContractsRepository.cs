using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Contracts.Application.Clients
{
    public class ContractsRepository
    {
        private readonly IContractsStore _contractsStore;
        private readonly ContractsRightsFilter _rightsFilter;
        private readonly ClaimsPrincipal _principal;

        public ContractsRepository(IContractsStore contractsStore, ContractsRightsFilter rightsFilter, ClaimsPrincipal principal)
        {
            _contractsStore = contractsStore;
            _rightsFilter = rightsFilter;
            _principal = principal;
        }

        public async Task<Page<Contract>> GetPageAsync(IPageToken pageToken, ContractFilter contractFilter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _contractsStore.GetPageAsync(accessRight, contractFilter, pageToken);
        }

        public async Task<List<Contract>> GetAsync(ContractFilter contractFilter)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _contractsStore.GetAsync(accessRight, contractFilter);
        }

        public async Task<ContractComment> GetCommentAsync(int contractId)
        {
            var accessRight = await _rightsFilter.GetReadAccessAsync(_principal);
            var comment = await _contractsStore.GetCommentAsync(accessRight, contractId);
            return comment ?? throw new NotFoundException();
        }
    }
}
