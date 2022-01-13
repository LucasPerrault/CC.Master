using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Lock;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Tools;

namespace Billing.Contracts.Application
{
    public class CountProcessService
    {
        private readonly IContractsStore _contractsStore;
        private readonly ICountsStore _countsStore;
        private readonly ITeamNotifier _teamNotifier;
        private readonly CountService _countService;
        private readonly ILockService _lockService;

        public CountProcessService
        (
            IContractsStore contractsStore,
            ICountsStore countsStore,
            ITeamNotifier teamNotifier,
            CountService countService,
            ILockService lockService
        )
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _teamNotifier = teamNotifier;
            _countService = countService;
            _lockService = lockService;
        }

        public async Task<CountProcessResult> RunAsync(AccountingPeriod period)
        {
            using (await _lockService.TakeLockAsync("CountProcess", TimeSpan.FromSeconds(5)))
            {
                var contractsWithCount = await GetContractsWithCountAsync(period);
                var totalContractCount = contractsWithCount.Count;
                var contractsWithMissingCount = contractsWithCount.Where(c => c.Count is null).ToList();
                var alreadyProcessedContracts = totalContractCount - contractsWithMissingCount.Count;

                await _teamNotifier.NotifyAsync(Team.CountProcessFollowers, $":flying_money_with_wings: Processus de décomptes : ${totalContractCount} contrats pour la période ${period:YYYY-MM}, ${alreadyProcessedContracts} déjà traités");

                return await _countService.CreateForPeriodAsync
                (
                    period,
                    contractsWithMissingCount.Select(c => c.Contract).ToList()
                );
            }
        }

        private async Task<List<ContractWithCount>> GetContractsWithCountAsync(AccountingPeriod period)
        {
            var contractsFilter = new ContractFilter
            {
                StartsOn = CompareDateTime.IsBeforeOrEqual(period),
                EndsOn = CompareDateTime.IsStrictlyAfter(period).OrNull(),
                ArchivedAt = CompareDateTime.IsStrictlyAfter(period).OrNull(),
            };

            var countsFilter = new CountFilter
            {
                CountPeriod = CompareDateTime.IsEqual(period),
            };

            var contracts = await _contractsStore.GetAsync(AccessRight.All, contractsFilter);
            var counts = await _countsStore.GetAsync(AccessRight.All, countsFilter);

            var contractWithCounts = new List<ContractWithCount>();

            var countsPerContractId = counts
                .GroupBy(c => c.ContractId)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Single());

            foreach (var contract in contracts)
            {
                countsPerContractId.TryGetValue(contract.Id, out var count);
                contractWithCounts.Add(new ContractWithCount { Contract = contract, Count = count });
            }

            return contractWithCounts;
        }

        private class ContractWithCount
        {
            public Contract Contract { get; set; }
            public Count Count { get; set; }
        }
    }
}
