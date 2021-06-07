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
        private readonly IContractAnalyticSituationsService _analyticSituationsService;

        private bool IsFirstDayOfMonth(DateTime date) => date.Day == 1;
        public CmrrSituationsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore, IContractAnalyticSituationsService analyticSituationsService)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _analyticSituationsService = analyticSituationsService;
        }

        public async Task<CmrrSituation> GetSituationAsync(CmrrSituationFilter situationFilter)
        {
            var lines = await GetLinesAsync(situationFilter);
            var situation = new CmrrSituation
            {
                Lines = lines,
                Axis = situationFilter.Axis,
                StartPeriod = situationFilter.StartPeriod,
                EndPeriod = situationFilter.EndPeriod
            };

            return situation;
        }

        private async Task<List<CmrrLine>> GetLinesAsync(CmrrSituationFilter situationFilter)
        {
            var contractSituations = await GetContractSituationsAsync(situationFilter);

            var orderedAnalyticSituations = await _analyticSituationsService.GetOrderedSituationsAsync(situationFilter.Axis, contractSituations);

            return orderedAnalyticSituations.Select(GetCmrrLines).ToList();
        }

        private async Task<List<CmrrContractSituation>> GetContractSituationsAsync(CmrrSituationFilter situationFilter)
        {
            ThrowIfDatesInvalid(situationFilter.StartPeriod, situationFilter.EndPeriod);

            var startPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.StartPeriod);
            var endPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.EndPeriod);

            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(situationFilter.StartPeriod, situationFilter.EndPeriod);

            if (situationFilter.ClientId.Any())
                contracts = contracts.Where(c => situationFilter.ClientId.Contains(c.ClientId));

            if (situationFilter.DistributorsId.Any())
                contracts = contracts.Where(c => situationFilter.DistributorsId.Contains(c.DistributorId));

            return CreateContractSituations(contracts, startPeriodCounts, endPeriodCounts).ToList();
        }

        private void ThrowIfDatesInvalid(DateTime startPeriod, DateTime endPeriod)
        {
            if (!IsFirstDayOfMonth(startPeriod))
                throw new ArgumentException($"{nameof(startPeriod)} must be on the first day of month");

            if (!IsFirstDayOfMonth(endPeriod))
                throw new ArgumentException($"{nameof(endPeriod)} must be on the first day of month");
        }

        private static IEnumerable<CmrrContractSituation> CreateContractSituations(IEnumerable<CmrrContract> contracts, List<CmrrCount> startPeriodCounts, List<CmrrCount> endPeriodCounts)
        {
            var startPeriodCountsByContractId = startPeriodCounts.ToDictionary(c => c.ContractId, c => c);
            var endPeriodCountsByContractId = endPeriodCounts.ToDictionary(c => c.ContractId, c => c);

            foreach (var contract in contracts)
            {
                startPeriodCountsByContractId.TryGetValue(contract.Id, out var startPeriodCount);

                endPeriodCountsByContractId.TryGetValue(contract.Id, out var endPeriodCount);

                if (startPeriodCount is null && endPeriodCount is null)
                    continue;

                yield return new CmrrContractSituation(contract, startPeriodCount, endPeriodCount);
            }
        }

        private CmrrLine GetCmrrLines(IGrouping<AxisSection, ContractAxisSectionSituation> group)
        {
            var line = new CmrrLine(group.Key.Name);

            foreach (var analyticSituation in group)
            {
                var subSectionName = analyticSituation.Breakdown.SubSection;
                line.SubSections.TryAdd(subSectionName, new CmrrAxisSection(subSectionName));
                var subSection = line.SubSections[subSectionName];
                Add(subSection, analyticSituation);
            }

            return line;
        }

        private void Add(CmrrAxisSection section, ContractAxisSectionSituation situation)
        {
            switch (situation.ContractSituation.LifeCycle)
            {
                case CmrrLifeCycle.Creation:
                    UpdateCmrrAmount(section.Creation, situation, s => s.PartialDiff);
                    break;
                case CmrrLifeCycle.Expansion:
                    UpdateCmrrAmount(section.Expansion, situation, s => s.PartialDiff);
                    break;
                case CmrrLifeCycle.Retraction:
                    UpdateCmrrAmount(section.Retraction, situation, s => s.PartialDiff);
                    break;
                case CmrrLifeCycle.Termination:
                    UpdateCmrrAmount(section.Termination, situation, s => s.PartialDiff);
                    break;
                case CmrrLifeCycle.Upsell:
                    UpdateCmrrAmount(section.Upsell, situation, s => s.PartialDiff);
                    break;
            }

            UpdateCmrrAmount(section.TotalFrom, situation, s => s.StartPeriodAmount);
            UpdateCmrrAmount(section.TotalTo, situation, s => s.EndPeriodAmount);
        }

        private void UpdateCmrrAmount(CmrrAmount amount, ContractAxisSectionSituation axisSectionSituation, Func<ContractAxisSectionSituation, decimal> amountFunc)
        {
            amount.Amount += amountFunc(axisSectionSituation);
            if (amount.Top.Count < CmrrAmountTopElement.TopCount)
                amount.Top.Add(CmrrAmountTopElement.FromSituation(axisSectionSituation));
        }
    }
}
