using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tools;

namespace Billing.Cmrr.Application
{
    public class CmrrEvolutionsService : ICmrrEvolutionsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;
        private readonly ICmrrRightsFilter _cmrrRightsFilter;
        private readonly IContractAxisSectionSituationsService _axisSectionSituationsService;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public CmrrEvolutionsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore, ICmrrRightsFilter cmrrRightsFilter, IContractAxisSectionSituationsService axisSectionSituationsService, ClaimsPrincipal claimsPrincipal)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _cmrrRightsFilter = cmrrRightsFilter;
            _axisSectionSituationsService = axisSectionSituationsService;
            _claimsPrincipal = claimsPrincipal;
        }


        public async Task<CmrrEvolution> GetEvolutionAsync(CmrrFilter filter)
        {
            var lines = await GetLinesAsync(filter).ToListAsync();

            var evolution = new CmrrEvolution
            {
                Lines = lines,
                StartPeriod = filter.StartPeriod,
                EndPeriod = filter.EndPeriod
            };

            return evolution;
        }

        private async IAsyncEnumerable<CmrrEvolutionLine> GetLinesAsync(CmrrFilter  filter)
        {
            CmrrDateTimeHelper.ThrowIfDatesAreNotAtFirstDayOfMonth(filter.StartPeriod, filter.EndPeriod);

            var counts = await _countsStore.GetBetweenAsync(filter.StartPeriod.AddMonths(-1), filter.EndPeriod);

            if (filter.BillingStrategies.Any())
            {
                counts = counts.Where(c => filter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
            }

            var countsByCountKey = counts.ToDictionary(c => new CountKey(c.CountPeriod, c.ContractId), c => c);

            var contracts = await GetContractsAsync(filter);

            for (var i = 0; i < CmrrDateTimeHelper.MonthDifference(filter.EndPeriod, filter.StartPeriod) + 1; i++)
            {
                var currentCountPeriod = filter.StartPeriod.AddMonths(1 * i);
                var previousCountPeriod = currentCountPeriod.AddMonths(-1);
                var evolutionLine = await GetEvolutionLineAsync(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts, filter.Axis, filter.Sections);
                yield return evolutionLine;
            }
        }

        private async Task<List<CmrrContract>> GetContractsAsync(CmrrFilter filter)
        {
            var accessRight = await _cmrrRightsFilter.GetReadAccessAsync(_claimsPrincipal);
            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(filter.StartPeriod.AddMonths(-1), filter.EndPeriod, accessRight);

            if (filter.ClientId.Any())
                contracts = contracts.Where(c => filter.ClientId.Contains(c.ClientId));

            if (filter.DistributorsId.Any())
                contracts = contracts.Where(c => filter.DistributorsId.Contains(c.DistributorId));

            if (filter.BillingEntities.Any())
                contracts = contracts.Where(c => filter.BillingEntities.Contains(c.ClientBillingEntity));

            return contracts.ToList();
        }

        private async Task<CmrrEvolutionLine> GetEvolutionLineAsync(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts, CmrrAxis axis, HashSet<string> selectedSections)
        {
            var line = new CmrrEvolutionLine(currentCountPeriod);
            var situations = GetEvolutionLineContractSituations(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts);

            var hasSections = selectedSections.Any();
            foreach (var situation in await _axisSectionSituationsService.GetAxisSectionSituationsAsync(axis, situations))
            {
                if (hasSections && !selectedSections.Contains(situation.Breakdown.AxisSection.Name))
                {
                    continue;
                }
                line.Amount += situation.EndPeriodAmount;
                GetLifeCycleIncrementFunc(situation.ContractSituation.LifeCycle)(situation.PartialDiff)(line);
            }
            return line;
        }

        private IEnumerable<CmrrContractSituation> GetEvolutionLineContractSituations(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts)
        {
            foreach (var contract in contracts)
            {
                countsByCountKey.TryGetValue(new CountKey(previousCountPeriod, contract.Id), out var previousPeriodCount);
                countsByCountKey.TryGetValue(new CountKey(currentCountPeriod, contract.Id), out var currentPeriodCount);

                if (currentPeriodCount is null && previousPeriodCount is null)
                    continue;

                yield return new CmrrContractSituation(contract, previousPeriodCount, currentPeriodCount);
            }
        }

        private Func<decimal, Action<CmrrEvolutionLine>> GetLifeCycleIncrementFunc(CmrrLifeCycle lifeCycle)
        {
            return lifeCycle switch
            {
                CmrrLifeCycle.Upsell => d => l => l.Upsell += d,
                CmrrLifeCycle.Creation => d => l => l.Creation += d,
                CmrrLifeCycle.Expansion => d => l => l.Expansion += d,
                CmrrLifeCycle.Contraction => d => l => l.Contraction += d,
                CmrrLifeCycle.Termination => d => l => l.Termination += d,
                _ => throw new InvalidEnumArgumentException(nameof(lifeCycle), (int)lifeCycle, typeof(CmrrLifeCycle))
            };
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
