using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts.CreationStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts
{
    public class CountService
    {
        private readonly CountCreationStrategyService _countCreationStrategyService;
        private readonly ICountRemoteService _countRemoteService;
        private readonly IEnvironmentGroupStore _environmentGroupStore;

        public CountService
        (
            ICountRemoteService countRemoteService,
            IEnvironmentGroupStore environmentGroupStore,
            CountCreationStrategyService countCreationStrategyService
        )
        {
            _countRemoteService = countRemoteService;
            _environmentGroupStore = environmentGroupStore;
            _countCreationStrategyService = countCreationStrategyService;
        }

        public async Task<IEnumerable<Count>> CreateForPeriodAsync(AccountingPeriod countPeriod, List<Contract> contractsToCount)
        {
            var contractsWithoutGroup = contractsToCount.Where(c => !(c.Environment?.GroupId).HasValue).ToList();
            if (contractsWithoutGroup.Any())
            {
                throw new ApplicationException($"Contracts cannot be processed (env group is missing) : {string.Join(',', contractsWithoutGroup.Select(c => c.Id))}");
            }

            var contractsPerEnvGroup =  contractsToCount
                .GroupBy(c => c.Environment.GroupId)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Select(c => c.Id).ToHashSet());

            var envGroups = await _environmentGroupStore.GetEnvGroupsAsync(contractsPerEnvGroup.Keys);

            var tasks = envGroups.Select(g => CreateForPeriodAsync(g, contractsPerEnvGroup[g.EnvironmentGroupId], countPeriod));
            var counts = await Task.WhenAll(tasks);

            return counts.SelectMany(c => c);
        }

        public async Task<IEnumerable<Count>> CreateForPeriodAsync
        (
            EnvironmentWithContractGroup environmentWithContractGroup,
            HashSet<int> contractsToCountIds,
            AccountingPeriod countPeriod
        )
        {
            ThrowIfContractsAreNotInEnvironmentGroup(environmentWithContractGroup, contractsToCountIds);
            var productIds = GetProductIdsToCount(environmentWithContractGroup, contractsToCountIds);

            using var remoteCountContext = _countRemoteService.GetCountContext();
            var contractsWithCountNumberTasks = environmentWithContractGroup.Contracts
                .Where(c => productIds.Contains(c.CommercialOffer.ProductId))
                .Select(c => remoteCountContext.GetNumberFromRemoteAsync(c, countPeriod))
                .ToList();

            var contractsWithCountNumber = await Task.WhenAll(contractsWithCountNumberTasks);

            return await GetCounts(countPeriod, contractsToCountIds, contractsWithCountNumber);
        }

        private async Task<IEnumerable<Count>> GetCounts(AccountingPeriod countPeriod, HashSet<int> contractsToCountIds, ContractWithCountNumber[] contractsWithCountNumber)
        {
            var contractsWithCountNumberPerProductId = contractsWithCountNumber
                .GroupBy(c => c.Contract.CommercialOffer.ProductId)
                .ToDictionary(kvp => kvp.First().Contract.CommercialOffer.ProductId, kvp => kvp.ToList());

            var contractsWithCountNumberToCreateCounts = contractsWithCountNumber
                .Where(c => contractsToCountIds.Contains(c.Contract.Id));

            var counts = new List<Count>();
            foreach (var contractWithCountNumber in contractsWithCountNumberToCreateCounts)
            {
                var otherFromContractGroup = contractsWithCountNumberPerProductId[contractWithCountNumber.Contract.CommercialOffer.ProductId]
                    .Where(c => c.Contract.Id != contractWithCountNumber.Contract.Id)
                    .ToList();

                var strategy = _countCreationStrategyService.GetCountCreationStrategy(contractWithCountNumber);
                var count = await strategy.MakeCountAsync(countPeriod, contractWithCountNumber, otherFromContractGroup);
                count.Details = contractWithCountNumber.Details;
                counts.Add(count);
            }

            return counts;
        }

        private HashSet<int> GetProductIdsToCount(EnvironmentWithContractGroup environmentWithContractGroup, HashSet<int> contractsToCountIds)
        {
            return environmentWithContractGroup.Contracts
                .Where(c => contractsToCountIds.Contains(c.Id))
                .Select(c => c.CommercialOffer.ProductId)
                .ToHashSet();
        }

        private void ThrowIfContractsAreNotInEnvironmentGroup(EnvironmentWithContractGroup environmentWithContractGroup, HashSet<int> contractsToCountIds)
        {
            var contractGroupIds = environmentWithContractGroup.Contracts.Select(c => c.Id);
            var contractFromOtherEnvironmentsGroup = contractsToCountIds.Except(contractGroupIds).ToList();
            if (contractFromOtherEnvironmentsGroup.Any())
            {
                throw new ApplicationException($"Some contracts are not in same group id : {string.Join(',', contractFromOtherEnvironmentsGroup)}");
            }
        }
    }
}
