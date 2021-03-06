using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Offers.Interfaces;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Domain.Offers.Services
{
    public class OfferUsageContract
    {
        public int CommercialOfferId { get; set; }
        public ContractStatus Status { get; set; }
    }

    public class CommercialOfferUsageService : ICommercialOfferUsageService
    {
        private readonly IContractsStore _contractsStore;
        private readonly ICountsStore _countsStore;
        private readonly ITimeProvider _time;

        public CommercialOfferUsageService(IContractsStore contractsStore, ICountsStore countsStore, ITimeProvider time)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _time = time;
        }

        public async Task<CommercialOfferUsage> BuildAsync(int offerId)
        {
            return (await BuildAsync(new HashSet<int> { offerId })).SingleOrDefault();
        }

        public async Task<IReadOnlyCollection<CommercialOfferUsage>> BuildAsync(HashSet<int> offerIds, AccessRight accessRight)
        {
            if (!HasRight(accessRight))
            {
                return new List<CommercialOfferUsage>();
            }

            return await BuildAsync(offerIds);
        }

        private async Task<IReadOnlyCollection<CommercialOfferUsage>> BuildAsync(HashSet<int> offerIds)
        {
            var contractsByOfferId = (await _contractsStore
                .GetOfferUsageContractAsync(AccessRight.All, GetContractFilter(offerIds)))
                .GroupBy(c => c.CommercialOfferId);

            var counts = await _countsStore
                .GetAsync(new CountFilter { CommercialOfferIds = offerIds });

            var mostRecentCountPeriod = counts
                .Select(c => c.CountPeriod)
                .Distinct()
                .OrderByDescending(period => period)
                .FirstOrDefault();

            var nbCountedContractsByOfferId = counts
                .GroupBy(c => c.CommercialOfferId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.ContractId).Distinct().Count());

            return contractsByOfferId
                .Select(kvp => new CommercialOfferUsage
                {
                    OfferId = kvp.Key,
                    NumberOfContracts = kvp.Count(),
                    NumberOfActiveContracts = kvp.Count(c => c.Status == ContractStatus.InProgress),
                    NumberOfNotStartedContracts = kvp.Count(c => c.Status == ContractStatus.NotStarted),
                    NumberOfCountedContracts = nbCountedContractsByOfferId.ContainsKey(kvp.Key) ? nbCountedContractsByOfferId[kvp.Key] : 0,
                    MostRecentCountPeriod = mostRecentCountPeriod,
                })
                .ToList();
        }

        private ContractFilter GetContractFilter(HashSet<int> offerIds, ContractStatus? status = null)
        {
            var filter = new ContractFilter
            {
                CommercialOfferIds = offerIds,
                ArchivedAt = CompareDateTime.IsStrictlyAfter(_time.Now()).OrNull()
            };

            if (status.HasValue)
            {
                filter.ContractStatuses = new HashSet<ContractStatus> { status.Value };
            }

            return filter;
        }

        private static bool HasRight(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => false,
                AllAccessRight _ => true,
                _ => throw new ApplicationException($"Unknown type of offer filter right {accessRight}")
            };
        }
    }
}
