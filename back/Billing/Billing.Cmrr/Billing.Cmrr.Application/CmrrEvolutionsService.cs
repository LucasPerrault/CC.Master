using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Billing.Cmrr.Application
{
    public class CmrrEvolutionsService : ICmrrEvolutionsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;


        public CmrrEvolutionsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
        }


        public async Task<CmrrEvolution> GetEvolutionAsync(CmrrEvolutionFilter evolutionFilter)
        {
            var lines = await GetLinesAsync(evolutionFilter).ToListAsync();

            var evolution = new CmrrEvolution
            {
                Lines = lines,
                StartPeriod = evolutionFilter.StartPeriod,
                EndPeriod = evolutionFilter.EndPeriod
            };

            return evolution;
        }

        private async IAsyncEnumerable<CmrrEvolutionLine> GetLinesAsync(CmrrEvolutionFilter evolutionFilter)
        {
            CmrrDateTimeHelper.ThrowIfDatesAreNotAtFirstDayOfMonth(evolutionFilter.StartPeriod, evolutionFilter.EndPeriod);

            var counts = await _countsStore.GetBetweenAsync(evolutionFilter.StartPeriod.AddMonths(-1), evolutionFilter.EndPeriod);

            if (evolutionFilter.BillingStrategies.Any())
            {
                counts = counts.Where(c => evolutionFilter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
            }

            var countsByCountKey = counts.ToDictionary(c => new CountKey(c.CountPeriod, c.ContractId), c => c);

            var contracts = await GetContractsAsync(evolutionFilter);

            for (var i = 0; i < CmrrDateTimeHelper.MonthDifference(evolutionFilter.EndPeriod, evolutionFilter.StartPeriod) + 1; i++)
            {
                var currentCountPeriod = evolutionFilter.StartPeriod.AddMonths(1 * i);
                var previousCountPeriod = currentCountPeriod.AddMonths(-1);
                var evolutionLine = GetEvolutionLine(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts);
                yield return evolutionLine;
            }
        }

        private async Task<List<CmrrContract>> GetContractsAsync(CmrrEvolutionFilter evolutionFilter)
        {
            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(evolutionFilter.StartPeriod.AddMonths(-1), evolutionFilter.EndPeriod);

            if (evolutionFilter.ClientId.Any())
                contracts = contracts.Where(c => evolutionFilter.ClientId.Contains(c.ClientId));

            if (evolutionFilter.DistributorsId.Any())
                contracts = contracts.Where(c => evolutionFilter.DistributorsId.Contains(c.DistributorId));

            return contracts.ToList();
        }

        private CmrrEvolutionLine GetEvolutionLine(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts)
        {
            var line = new CmrrEvolutionLine(currentCountPeriod);

            foreach (var contract in contracts)
            {
                countsByCountKey.TryGetValue(new CountKey(previousCountPeriod, contract.Id), out var previousPeriodCount);
                countsByCountKey.TryGetValue(new CountKey(currentCountPeriod, contract.Id), out var currentPeriodCount);

                if (currentPeriodCount is null && previousPeriodCount is null)
                    continue;

                var contractSituation = new CmrrContractSituation(contract, previousPeriodCount, currentPeriodCount);
                line.Amount += contractSituation.EndPeriodCount?.EuroTotal ?? 0;

                var diff = (contractSituation.EndPeriodCount?.EuroTotal ?? 0) - (contractSituation.StartPeriodCount?.EuroTotal ?? 0);
                ApplyAmountAccordingToLifeCycle(line, contractSituation.LifeCycle, diff);
            }
            return line;
        }

        private void ApplyAmountAccordingToLifeCycle(CmrrEvolutionLine line, CmrrLifeCycle lifeCycle, decimal amount)
        {
            switch (lifeCycle)
            {
                case CmrrLifeCycle.Upsell:
                    line.Upsell += amount;
                    break;
                case CmrrLifeCycle.Creation:
                    line.Creation += amount;
                    break;
                case CmrrLifeCycle.Expansion:
                    line.Expansion += amount;
                    break;
                case CmrrLifeCycle.Contraction:
                    line.Contraction += amount;
                    break;
                case CmrrLifeCycle.Termination:
                    line.Termination += amount;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(lifeCycle), (int)lifeCycle, typeof(CmrrLifeCycle));
            }
        }


        private class CountKey : ValueObject
        {
            public DateTime Period { get; }
            public int ContractId { get; }

            public CountKey(DateTime period, int contractId)
            {
                Period = period;
                ContractId = contractId;
            }
            protected override IEnumerable<object> EqualityComponents
            {
                get
                {
                    yield return Period;
                    yield return ContractId;
                }
            }
        }
    }
}
