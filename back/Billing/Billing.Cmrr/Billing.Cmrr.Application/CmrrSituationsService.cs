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

        public CmrrSituationsService(
            ICmrrContractsStore contractsStore,
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

        public async Task<CmrrSituation> GetSituationAsync(CmrrFilter filter)
        {
            var contractSituations = await GetContractSituationsAsync(filter);

            var axisSectionSituations = (await _axisSectionSituationsService
                .GetAxisSectionSituationsAsync(filter.Axis, contractSituations))
                .ToList();

            var sections = axisSectionSituations
                .Select(s => s.Breakdown.AxisSection)
                .Distinct()
                .Where(s => !filter.Sections.Any() || filter.Sections.Contains(s.Name))
                .OrderBy(s => s.Order);

            var situation = new CmrrSituation
            {
                Axis = filter.Axis,
                StartPeriod = filter.StartPeriod,
                EndPeriod = filter.EndPeriod,
                Total = new CmrrSubLine("Total"),
                Lines = sections.Select(s => new CmrrLine(s.Name)).ToList(),
                Clients = new CmrrClientSituation(),
            };

            PopulateSituation(situation, axisSectionSituations);

            return situation;
        }



        private async Task<List<CmrrContractSituation>> GetContractSituationsAsync(CmrrFilter filter)
        {
            CmrrDateTimeHelper.ThrowIfDatesAreNotAtFirstDayOfMonth(filter.StartPeriod, filter.EndPeriod);

            var counts = await _countsStore.GetByPeriodsAsync(filter.StartPeriod, filter.EndPeriod);

            var startPeriodCounts = counts.Where(c => c.CountPeriod == filter.StartPeriod);
            var endPeriodCounts = counts.Where(c => c.CountPeriod == filter.EndPeriod);

            if (filter.BillingStrategies.Any())
            {
                startPeriodCounts = startPeriodCounts.Where(c => filter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
                endPeriodCounts = endPeriodCounts.Where(c => filter.BillingStrategies.Contains(c.BillingStrategy)).ToList();
            }

            var accessRight = await _cmrrRightsFilter.GetReadAccessAsync(_claimsPrincipal);
            IEnumerable<CmrrContract> contracts = await _contractsStore.GetContractsNotEndedAtAsync(filter.StartPeriod, filter.EndPeriod, accessRight);

            if (filter.ClientId.Any())
                contracts = contracts.Where(c => filter.ClientId.Contains(c.ClientId));

            if (filter.DistributorsId.Any())
                contracts = contracts.Where(c => filter.DistributorsId.Contains(c.DistributorId));

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
            var linesPerName = cmrrSituation.Lines.ToDictionary(l => l.Name, l => l);

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.PartialDiff)))
            {
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
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
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
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
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                UpdateEndAmount(subLine.TotalTo, situation);
                UpdateEndAmount(line.Total.TotalTo, situation);
                UpdateEndAmount(cmrrSituation.Total.TotalTo, situation);
            }

            PopulateClientsSituation(cmrrSituation, situations, linesPerName.Keys);
        }

        private static void PopulateClientsSituation(CmrrSituation cmrrSituation, IReadOnlyCollection<ContractAxisSectionSituation> situations, Dictionary<string, CmrrLine>.KeyCollection sections)
        {
            foreach (var situationGroup in situations.Where(s => sections.Contains(s.Breakdown.AxisSection.Name)).GroupBy(s => s.ContractSituation.Contract.ClientId))
            {
                if (situationGroup.All(c => c.ContractSituation.StartPeriodCount == null))
                {
                    cmrrSituation.Clients.Acquired.Add(new CmrrClient { Name = situationGroup.First().ContractSituation.Contract.ClientName });
                }

                if (situationGroup.All(c => c.ContractSituation.EndPeriodCount == null))
                {
                    cmrrSituation.Clients.Terminated.Add(new CmrrClient { Name = situationGroup.First().ContractSituation.Contract.ClientName });
                }
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
