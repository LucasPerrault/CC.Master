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
        public async Task<List<CmrrContratSituation>> GetContractSituationsAsync(CmrrSituationFilter situationFilter)
        {
            ThrowIfDatesInvalid(situationFilter.StartPeriod, situationFilter.EndPeriod);

            var startPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.StartPeriod);
            var endPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.EndPeriod);
            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(situationFilter.StartPeriod, situationFilter.EndPeriod);

            if (situationFilter.ClientId.Any())
                contracts = contracts.Where(c => situationFilter.ClientId.Contains(c.ClientId));

            if (situationFilter.DistributorsId.Any())
                contracts = contracts.Where(c => situationFilter.DistributorsId.Contains(c.DistributorId));

            var situations = CreateContractSituations(contracts, startPeriodCounts, endPeriodCounts);

            return situations;
        }

        private static List<CmrrContratSituation> CreateContractSituations(IEnumerable<CmrrContract> contracts, List<CmrrCount> startPeriodCounts, List<CmrrCount> endPeriodCounts)
        {
            var situations = new List<CmrrContratSituation>();

            var startPeriodCountsByContractId = startPeriodCounts.ToDictionary(c => c.ContractId, c => c);
            var endPeriodCountsByContractId = endPeriodCounts.ToDictionary(c => c.ContractId, c => c);

            foreach (var contract in contracts)
            {
                var situation = new CmrrContratSituation
                {
                    Contract = contract,
                    ContractId = contract.Id
                };

                if (startPeriodCountsByContractId.TryGetValue(contract.Id, out var startPeriodCount))
                    situation.StartPeriodCount = startPeriodCount;

                if (endPeriodCountsByContractId.TryGetValue(contract.Id, out var endPeriodCount))
                    situation.EndPeriodCount = endPeriodCount;

                if (situation.StartPeriodCount is null && situation.EndPeriodCount is null)
                    continue;

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
                throw new ArgumentException($"{nameof(startPeriod)} must be on the first day of month");

            if (!IsFirstDayOfMonth(endPeriod))
                throw new ArgumentException($"{nameof(endPeriod)} must be on the first day of month");
        }
    }
}
