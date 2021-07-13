using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public class CmrrSituationsService : ICmrrSituationsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;
        private readonly IContractAxisSectionSituationsService _axisSectionSituationsService;
        private readonly ICmrrRightsFilter _cmrrRightsFilter;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public CmrrSituationsService(ICmrrContractsStore contractsStore,
            ICmrrCountsStore countsStore,
            IContractAxisSectionSituationsService axisSectionSituationsService,
            ICmrrRightsFilter cmrrRightsFilter,
            ClaimsPrincipal claimsPrincipal)
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _axisSectionSituationsService = axisSectionSituationsService;
            _cmrrRightsFilter = cmrrRightsFilter;
            _claimsPrincipal = claimsPrincipal;
        }

        public async Task<CmrrSituation> GetSituationAsync(CmrrSituationFilter situationFilter)
        {
            var contractSituations = await GetContractSituationsAsync(situationFilter);

            var axisSectionSituations = (await _axisSectionSituationsService
                .GetAxisSectionSituationsAsync(situationFilter.Axis, contractSituations))
                .ToList();

            var sections = axisSectionSituations
                .Select(s => s.Breakdown.AxisSection)
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

            var counts = await _countsStore.GetByPeriodsAsync(situationFilter.StartPeriod, situationFilter.EndPeriod);

            var startPeriodCounts = counts.Where(c => c.CountPeriod == situationFilter.StartPeriod);
            var endPeriodCounts = counts.Where(c => c.CountPeriod == situationFilter.EndPeriod);

            if (situationFilter.BillingStrategies.Any())
            {
                startPeriodCounts = startPeriodCounts.Where(c => situationFilter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
                endPeriodCounts = endPeriodCounts.Where(c => situationFilter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
            }

            var accessRight = await _cmrrRightsFilter.GetReadAccessAsync(_claimsPrincipal);
            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(situationFilter.StartPeriod, situationFilter.EndPeriod, accessRight);

            if (situationFilter.ClientId.Any())
                contracts = contracts.Where(c => situationFilter.ClientId.Contains(c.ClientId));

            if (situationFilter.DistributorsId.Any())
                contracts = contracts.Where(c => situationFilter.DistributorsId.Contains(c.DistributorId));

            return CreateContractSituations(contracts, startPeriodCounts, endPeriodCounts).ToList();
        }

        private static IEnumerable<CmrrContractSituation> CreateContractSituations(IEnumerable<CmrrContract> contracts, IEnumerable<CmrrCount> startPeriodCounts, IEnumerable<CmrrCount> endPeriodCounts)
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
                var line = cmrrSituation.Lines.SingleOrDefault(l => l.Name == situation.Breakdown.AxisSection.Name);
                if (line is null)
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                var amount = GetAmount(subLine, situation);
                UpdateLifecycleAmount(amount, situation);

                var totalAmount = GetAmount(line.Total, situation);
                UpdateLifecycleAmount(totalAmount, situation);

                var grandTotalAmount = GetAmount(cmrrSituation.Total, situation);
                UpdateLifecycleAmount(grandTotalAmount, situation);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.StartPeriodAmount)))
            {
                var line = cmrrSituation.Lines.SingleOrDefault(l => l.Name == situation.Breakdown.AxisSection.Name);
                if (line is null)
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                UpdateStartAmount(subLine.TotalFrom, situation);
                UpdateStartAmount(line.Total.TotalFrom, situation);
                UpdateStartAmount(cmrrSituation.Total.TotalFrom, situation);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.EndPeriodAmount)))
            {
                var line = cmrrSituation.Lines.SingleOrDefault(l => l.Name == situation.Breakdown.AxisSection.Name);
                if (line is null)
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                UpdateEndAmount(subLine.TotalTo, situation);
                UpdateEndAmount(line.Total.TotalTo, situation);
                UpdateEndAmount(cmrrSituation.Total.TotalTo, situation);
            }
        }

        private CmrrSubLine GetSubLine(CmrrLine line, ContractAxisSectionSituation situation)
        {
            var subSectionName = situation.Breakdown.SubSection;
            line.SubLines.TryAdd(subSectionName, new CmrrSubLine(subSectionName));
            return line.SubLines[subSectionName];
        }

        private void UpdateLifecycleAmount(CmrrAmount amount, ContractAxisSectionSituation situation)
        {
            UpdateCmrrAmount(amount, situation, s => s.PartialDiff, s => s.UserCountDiff, _ => true);
        }

        private void UpdateStartAmount(CmrrAmount startAmount, ContractAxisSectionSituation situation)
        {
            UpdateCmrrAmount(startAmount, situation, s => s.StartPeriodAmount, s => s.StartPeriodUserCount, s => s.ContractSituation.StartPeriodCount != null);
        }

        private void UpdateEndAmount(CmrrAmount startAmount, ContractAxisSectionSituation situation)
        {
            UpdateCmrrAmount(startAmount, situation, s => s.EndPeriodAmount, s => s.EndPeriodUserCount, s => s.ContractSituation.EndPeriodCount != null);
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
