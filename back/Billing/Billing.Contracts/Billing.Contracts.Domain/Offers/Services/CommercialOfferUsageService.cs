using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Offers.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Services
{
    public class CommercialOfferUsageService : ICommercialOfferUsageService
    {
        private readonly IContractsStore _contractsStore;
        private readonly ICountsStore _countsStore;

        public CommercialOfferUsageService(IContractsStore contractsStore, ICountsStore countsStore)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
        }

        public Task<CommercialOfferUsage> BuildAsync(int offerId)
        {
            var nbContracts = _contractsStore
                .GetQueryable(GetContractFilter(offerId))
                .Count();
            var nbActiveContracts = _contractsStore
                .GetQueryable(GetContractFilter(offerId, ContractStatus.InProgress))
                .Count();
            var nbNotStartedContracts = _contractsStore
                .GetQueryable(GetContractFilter(offerId, ContractStatus.NotStarted))
                .Count();
            var nbContractsWithCount = _countsStore
                .GetQueryable(new CountFilter { CommercialOfferIds = new HashSet<int> { offerId } })
                .GroupBy(c => c.ContractId)
                .Count();

            return Task.FromResult(new CommercialOfferUsage
            {
                Id = offerId,
                NumberOfContracts = nbContracts,
                NumberOfActiveContracts = nbActiveContracts,
                NumberOfNotStartedContracts = nbNotStartedContracts,
                NumberOfCountedContracts = nbContractsWithCount
            });
        }

        private ContractFilter GetContractFilter(int offerId, ContractStatus? status = null)
        {
            var filter = new ContractFilter
            {
                CommercialOfferIds = new HashSet<int> { offerId }
            };

            if (status.HasValue)
            {
                filter.ContractStatuses = new HashSet<ContractStatus> { status.Value };
            }

            return filter;
        }
    }
}
