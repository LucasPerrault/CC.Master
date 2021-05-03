using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public class CmrrSituationsService : ICmrrSituationsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;

        private bool IsFirstDayOfMonth(DateTime date) => date.Day == 1;
        public CmrrSituationsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore)
        {
            _contractsStore = contractsStore ?? throw new ArgumentNullException(nameof(contractsStore));
            _countsStore = countsStore ?? throw new ArgumentNullException(nameof(countsStore));
        }
        public async Task<List<CmrrContratSituation>> GetContractSituationsAsync(DateTime startPeriod, DateTime endPeriod)
        {
            ThrowIfDatesInvalid(startPeriod, endPeriod);

            var startPeriodCounts = await _countsStore.GetByPeriodAsync(startPeriod);
            var endPeriodCounts = await _countsStore.GetByPeriodAsync(endPeriod);
            var contracts = await _contractsStore.GetContractsNotEndedAtAsync(startPeriod);


            var startPeriodCountsByContractId = startPeriodCounts.ToDictionary(c => c.ContractId, c => c);
            var endPeriodCountsByContractId = endPeriodCounts.ToDictionary(c => c.ContractId, c => c);
            var allCounts = new List<CmrrCount>();
            allCounts.AddRange(startPeriodCounts);
            allCounts.AddRange(endPeriodCounts);

            var situations = new List<CmrrContratSituation>();

            foreach (var contract in contracts)
            {
                var situation = new CmrrContratSituation
                {
                    Contract = contract,
                    ContractId = contract.Id
                };

                if (startPeriodCountsByContractId.TryGetValue(contract.Id, out var firstPeriodCount))
                    situation.FirstPeriodCount = firstPeriodCount;

                if (endPeriodCountsByContractId.TryGetValue(contract.Id, out var lastPeriodCount))
                    situation.LastPeriodCount = lastPeriodCount;

                situations.Add(situation);

            }

            return situations;
        }

        private void ThrowIfDatesInvalid(DateTime startPeriod, DateTime endPeriod)
        {
            if (startPeriod == default)
                throw new ArgumentNullException($"{nameof(startPeriod)} cannot be default value");

            if (endPeriod == default)
                throw new ArgumentNullException($"{nameof(endPeriod)} cannot be default value");

            if(!IsFirstDayOfMonth(startPeriod))
                throw new ArgumentException($"{nameof(startPeriod)} must be on the day one of month");

            if (!IsFirstDayOfMonth(endPeriod))
                throw new ArgumentException($"{nameof(endPeriod)} must be on the day one of month");
        }

        private CmrrContratSituation GetCmrrSituation(DateTime startPeriod, DateTime endPeriod, List<CmrrContract> contracts, IGrouping<int, CmrrCount> countGroup)
        {
            var currentContract = contracts.SingleOrDefault(c => c.Id == countGroup.Key);
            return new CmrrContratSituation
            {
                Contract = currentContract,
                ContractId = countGroup.Key,
                FirstPeriodCount = countGroup.FirstOrDefault(countGroup => countGroup.CountPeriod == startPeriod),
                LastPeriodCount = countGroup.FirstOrDefault(countGroup => countGroup.CountPeriod == endPeriod),
            };
        }
    }
}
