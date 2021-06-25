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

        public CmrrSituationsService(ICmrrContractsStore contractsStore, ICmrrCountsStore countsStore, IContractAxisSectionSituationsService axisSectionSituationsService)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _axisSectionSituationsService = axisSectionSituationsService;
        }

        public async Task<CmrrSituation> GetSituationAsync(CmrrSituationFilter situationFilter)
        {
            var contractSituations = await GetContractSituationsAsync(situationFilter);

            var axisSectionSituations = (await _axisSectionSituationsService
                .GetAxisSectionSituationsAsync(situationFilter.Axis, contractSituations))
                .ToList();

            var sections = axisSectionSituations
                .Select(situation => situation.Breakdown.AxisSection)
                .Distinct()
                .Where(s => !situationFilter.Sections.Any() || situationFilter.Sections.Contains(s.Name))
                .OrderBy(s => s.Order);

            var situation = new CmrrSituation
            {
                Axis = situationFilter.Axis,
                StartPeriod = situationFilter.StartPeriod,
                EndPeriod = situationFilter.EndPeriod,
                Total = new CmrrSubLine("Total"),
                Lines = sections.Select(s => new CmrrLine(s.Name)).ToList()
            };

            PopulateSituation(situation, axisSectionSituations);

            return situation;
        }

        private async Task<List<CmrrContractSituation>> GetContractSituationsAsync(CmrrSituationFilter situationFilter)
        {
            CmrrDateTimeHelper.ThrowIfDatesAreNotAtFirstDayOfMonth(situationFilter.StartPeriod, situationFilter.EndPeriod);

            var startPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.StartPeriod);
            var endPeriodCounts = await _countsStore.GetByPeriodAsync(situationFilter.EndPeriod);


            if (situationFilter.BillingStrategies.Any())
            {
                startPeriodCounts = startPeriodCounts.Where(c => situationFilter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
                endPeriodCounts = endPeriodCounts.Where(c => situationFilter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
            }

            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(situationFilter.StartPeriod, situationFilter.EndPeriod);

            if (situationFilter.ClientId.Any())
                contracts = contracts.Where(c => situationFilter.ClientId.Contains(c.ClientId));

            if (situationFilter.DistributorsId.Any())
                contracts = contracts.Where(c => situationFilter.DistributorsId.Contains(c.DistributorId));

            return CreateContractSituations(contracts, startPeriodCounts, endPeriodCounts).ToList();
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

        private void PopulateSituation(CmrrSituation cmrrSituation, IReadOnlyCollection<ContractAxisSectionSituation> situations)
        {
            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.PartialDiff)))
            {
                var line = cmrrSituation.Lines.Single(l => l.Name == situation.Breakdown.AxisSection.Name);
                var subLine = GetSubLine(line, situation);
                var amount = GetAmount(subLine, situation);
                UpdateCmrrAmount(amount, situation, s => s.PartialDiff, s => s.UserCountDiff, _ => true);

                var totalAmount = GetAmount(line.Total, situation);
                UpdateCmrrAmount(totalAmount, situation, s => s.PartialDiff, s => s.UserCountDiff, _ => true);

                var grandTotalAmount = GetAmount(cmrrSituation.Total, situation);
                UpdateCmrrAmount(grandTotalAmount, situation, s => s.PartialDiff, s => s.UserCountDiff, _ => true);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.StartPeriodAmount)))
            {
                var line = cmrrSituation.Lines.Single(l => l.Name == situation.Breakdown.AxisSection.Name);
                var subLine = GetSubLine(line, situation);
                UpdateCmrrAmount(subLine.TotalFrom, situation, s => s.StartPeriodAmount, s => s.StartPeriodUserCount, s => s.ContractSituation.StartPeriodCount != null);

                UpdateCmrrAmount(line.Total.TotalFrom, situation, s => s.StartPeriodAmount, s => s.StartPeriodUserCount, s => s.ContractSituation.StartPeriodCount != null);
                UpdateCmrrAmount(cmrrSituation.Total.TotalFrom, situation, s => s.StartPeriodAmount, s => s.StartPeriodUserCount, s => s.ContractSituation.StartPeriodCount != null);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.EndPeriodAmount)))
            {
                var line = cmrrSituation.Lines.Single(l => l.Name == situation.Breakdown.AxisSection.Name);
                var subLine = GetSubLine(line, situation);
                UpdateCmrrAmount(subLine.TotalTo, situation, s => s.EndPeriodAmount, s => s.EndPeriodUserCount, s => s.ContractSituation.EndPeriodCount != null);

                UpdateCmrrAmount(line.Total.TotalTo, situation, s => s.EndPeriodAmount, s => s.EndPeriodUserCount, s => s.ContractSituation.EndPeriodCount != null);
                UpdateCmrrAmount(cmrrSituation.Total.TotalTo, situation, s => s.EndPeriodAmount, s => s.EndPeriodUserCount, s => s.ContractSituation.EndPeriodCount != null);
            }
        }

        private CmrrSubLine GetSubLine(CmrrLine line, ContractAxisSectionSituation situation)
        {
            var subSectionName = situation.Breakdown.SubSection;
            line.SubLines.TryAdd(subSectionName, new CmrrSubLine(subSectionName));
            return line.SubLines[subSectionName];
        }

        private void UpdateCmrrAmount
         (
            CmrrAmount amount,
            ContractAxisSectionSituation axisSectionSituation,
            Func<ContractAxisSectionSituation, decimal> amountFunc,
            Func<ContractAxisSectionSituation, int> userCountFunc,
            Func<ContractAxisSectionSituation, bool> shouldCountClientAndContracts
        )
        {
            amount.Amount += amountFunc(axisSectionSituation);
            amount.UserCount += userCountFunc(axisSectionSituation);

            if (shouldCountClientAndContracts(axisSectionSituation))
            {
                amount.AddClient(axisSectionSituation.ContractSituation.Contract.ClientId);
                amount.AddContract(axisSectionSituation.ContractSituation.ContractId);
            }

            if (amount.Top.Count < CmrrAmountTopElement.TopCount)
                amount.Top.Add(CmrrAmountTopElement.FromSituation(axisSectionSituation, amountFunc(axisSectionSituation), userCountFunc(axisSectionSituation)));
        }

        private CmrrAmount GetAmount(CmrrSubLine section, ContractAxisSectionSituation situation)
        {
            return situation.ContractSituation.LifeCycle switch
            {
                CmrrLifeCycle.Creation => section.Creation,
                CmrrLifeCycle.Expansion => section.Expansion,
                CmrrLifeCycle.Contraction => section.Contraction,
                CmrrLifeCycle.Termination => section.Termination,
                CmrrLifeCycle.Upsell => section.Upsell,
                _ => throw new InvalidEnumArgumentException(nameof(situation.ContractSituation.LifeCycle), (int)situation.ContractSituation.LifeCycle, typeof(CmrrLifeCycle))
            };
        }
    }
}
