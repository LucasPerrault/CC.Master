using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public class CmrrSituationsService : ICmrrSituationsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;
        private readonly IContractAxisSectionSituationsService _axisSectionSituationsService;

        private bool IsFirstDayOfMonth(DateTime date) => date.Day == 1;
        public CmrrSituationsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore, IContractAxisSectionSituationsService axisSectionSituationsService)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _axisSectionSituationsService = axisSectionSituationsService;
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

            var orderedAnalyticSituations = await _axisSectionSituationsService
                .GetAxisSectionSituationsAsync(situationFilter.Axis, contractSituations);

            var groupedAnalyticsSituations = orderedAnalyticSituations
                .GroupBy(analyticSituation => analyticSituation.Breakdown.AxisSection)
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedAnalyticsSituations.Select(a => GetCmrrLines(a.Key, a.Value)).ToList();
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

        private CmrrLine GetCmrrLines(AxisSection axisSection, List<ContractAxisSectionSituation> situations)
        {
            var line = new CmrrLine(axisSection.Name);

            foreach (var situation in situations.OrderByDescending(s => s.PartialDiff))
            {
                var subSection = GetSubSection(line, situation);
                UpdateLifeCycleAmounts(subSection, situation, s => s.PartialDiff);
            }

            foreach (var situation in situations.OrderByDescending(s => s.StartPeriodAmount))
            {
                var subSection = GetSubSection(line, situation);
                UpdateCmrrAmount(subSection.TotalFrom, situation, s => s.StartPeriodAmount);
            }

            foreach (var situation in situations.OrderByDescending(s => s.EndPeriodAmount))
            {
                var subSection = GetSubSection(line, situation);
                UpdateCmrrAmount(subSection.TotalTo, situation, s => s.EndPeriodAmount);
            }

            return line;
        }

        private CmrrAxisSection GetSubSection(CmrrLine line, ContractAxisSectionSituation situation)
        {
            var subSectionName = situation.Breakdown.SubSection;
            line.SubSections.TryAdd(subSectionName, new CmrrAxisSection(subSectionName));
            return line.SubSections[subSectionName];
        }

        private void UpdateCmrrAmount(CmrrAmount amount, ContractAxisSectionSituation axisSectionSituation, Func<ContractAxisSectionSituation, decimal> amountFunc)
        {
            amount.Amount += amountFunc(axisSectionSituation);
            if (amount.Top.Count < CmrrAmountTopElement.TopCount)
                amount.Top.Add(CmrrAmountTopElement.FromSituation(axisSectionSituation));
        }

        private void UpdateLifeCycleAmounts( CmrrAxisSection section, ContractAxisSectionSituation situation, Func<ContractAxisSectionSituation, decimal> amountFunc)
        {
            switch (situation.ContractSituation.LifeCycle)
            {
                case CmrrLifeCycle.Creation:
                    UpdateCmrrAmount(section.Creation, situation, amountFunc);
                    break;
                case CmrrLifeCycle.Expansion:
                    UpdateCmrrAmount(section.Expansion, situation, amountFunc);
                    break;
                case CmrrLifeCycle.Retraction:
                    UpdateCmrrAmount(section.Retraction, situation, amountFunc);
                    break;
                case CmrrLifeCycle.Termination:
                    UpdateCmrrAmount(section.Termination, situation, amountFunc);
                    break;
                case CmrrLifeCycle.Upsell:
                    UpdateCmrrAmount(section.Upsell, situation, amountFunc);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(situation.ContractSituation.LifeCycle), (int)situation.ContractSituation.LifeCycle, typeof(CmrrLifeCycle));
            }
        }
    }
}
