using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Rights.Domain.Filtering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Domain.Counts.CreationStrategy
{
    public class LastRealMonthCountCreationStrategy : CountCreationStrategy
    {
        private readonly ICountsStore _countsStore;
        private readonly ConcurrentDictionary<CountKey, Count> _cache = new ConcurrentDictionary<CountKey, Count>();

        public LastRealMonthCountCreationStrategy(ICountsStore countsStore)
        {
            _countsStore = countsStore;
        }

        protected override async Task<CountCreationResult> MakeCountAsync(AccountingPeriod countPeriod, ContractWithCountNumber contractWithCountNumber, List<ContractWithCountNumber> otherFromContractGroup)
        {
            var lastMonth = countPeriod.AddMonth(-1);
            var countToFind = new CountKey { CountPeriod = lastMonth, ContractId = contractWithCountNumber.Contract.Id };
            if (_cache.TryGetValue(countToFind, out var count))
            {
                return MakeCount(countPeriod, count, contractWithCountNumber.Contract);
            }

            var counts = await _countsStore.GetAsync(AccessRight.All, new CountFilter { CountPeriod = CompareDateTime.IsEqual(lastMonth) });
            foreach (var lastPeriodCount in counts)
            {
                AddToCache(lastPeriodCount);
            }

            if (_cache.TryGetValue(countToFind, out var cachedCount))
            {
                return MakeCount(countPeriod, cachedCount, contractWithCountNumber.Contract);
            }

            throw new ApplicationException($"No count available on {lastMonth}");

        }

        private CountCreationResult MakeCount(AccountingPeriod countPeriod, Count olderCount, Contract contract) => CountCreationResult.Success
        (
            new Count
            {
                CountPeriod = countPeriod,
                Number = olderCount.Number,
                ContractId = contract.Id,
                FixedPrice = olderCount.FixedPrice,
                UnitPrice = olderCount.UnitPrice,
                CommercialOfferId = contract.CommercialOfferId,
                BillingStrategy = BillingStrategy.Standard,
            }
        );

        private void AddToCache(Count count)
        {
            _cache[new CountKey(count)] = count;

        }

        private class CountKey : ValueObject
        {
            public CountKey()
            { }

            public CountKey(Count count)
            {
                ContractId = count.ContractId;
                CountPeriod = count.CountPeriod;
            }

            public int ContractId { get; set; }
            public AccountingPeriod CountPeriod { get; set; }

            protected override IEnumerable<object> EqualityComponents
            {
                get
                {
                    yield return ContractId;
                    yield return CountPeriod;
                }
            }
        }
    }
}
