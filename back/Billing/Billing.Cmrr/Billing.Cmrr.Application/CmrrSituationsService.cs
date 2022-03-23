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

        public Task<CmrrSituation> GetSituationAsync(CmrrFilter filter)
        {
            return GetSituationAsync(filter, hasTopLimit: true);
        }

        public async Task<IReadOnlyCollection<CmrrClient>> GetAcquiredClientsAsync(CmrrFilter filter)
        {
            var situation = await GetSituationAsync(filter, hasTopLimit: false);

            return situation.Total.Creation.Top
                .Select(c => CmrrClient.FromCmrrAmountTopElement(c))
                .ToList();
        }

        public async Task<IReadOnlyCollection<CmrrClient>> GetTerminatedClientsAsync(CmrrFilter filter)
        {
            var situation = await GetSituationAsync(filter, hasTopLimit: false);

            return situation.Total.Termination.Top
                .Select(c => CmrrClient.FromCmrrAmountTopElement(c))
                .ToList();
        }

        private async Task<CmrrSituation> GetSituationAsync(CmrrFilter filter, bool hasTopLimit)
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

            var linesPerName = situation.Lines.ToDictionary(l => l.Name, l => l);
            PopulateSituation(situation, axisSectionSituations, linesPerName, hasTopLimit: hasTopLimit);
            PopulateClientsSituation(situation, axisSectionSituations, linesPerName, hasTopLimit);

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

            if (filter.BillingEntities.Any())
                contracts = contracts.Where(c => filter.BillingEntities.Contains(c.ClientBillingEntity));

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

        private void PopulateSituation(CmrrSituation cmrrSituation, IReadOnlyCollection<ContractAxisSectionSituation> situations, Dictionary<string, CmrrLine> linesPerName, bool hasTopLimit)
        {
            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.PartialDiff)))
            {
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                var amount = GetAmount(subLine, situation);
                UpdateLifecycleAmount(amount, situation, hasTopLimit);

                var totalAmount = GetAmount(line.Total, situation);
                UpdateLifecycleAmount(totalAmount, situation, hasTopLimit);

                var grandTotalAmount = GetAmount(cmrrSituation.Total, situation);
                UpdateLifecycleAmount(grandTotalAmount, situation, hasTopLimit);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.StartPeriodAmount)))
            {
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                UpdateStartAmount(subLine.TotalFrom, situation, hasTopLimit);
                UpdateStartAmount(line.Total.TotalFrom, situation, hasTopLimit);
                UpdateStartAmount(cmrrSituation.Total.TotalFrom, situation, hasTopLimit);
            }

            foreach (var situation in situations.OrderByDescending(s => Math.Abs(s.EndPeriodAmount)))
            {
                if (!linesPerName.TryGetValue(situation.Breakdown.AxisSection.Name, out var line))
                {
                    continue;
                }

                var subLine = GetSubLine(line, situation);
                UpdateEndAmount(subLine.TotalTo, situation, hasTopLimit);
                UpdateEndAmount(line.Total.TotalTo, situation, hasTopLimit);
                UpdateEndAmount(cmrrSituation.Total.TotalTo, situation, hasTopLimit);
            }
        }

        private void PopulateClientsSituation(CmrrSituation cmrrSituation, IReadOnlyCollection<ContractAxisSectionSituation> situations, Dictionary<string, CmrrLine> linesPerName, bool hasTopLimit)
        {
            var sections = linesPerName.Keys;
            var clientSituations = situations
                .Where(s => sections.Contains(s.Breakdown.AxisSection.Name))
                .GroupBy(s => s.ContractSituation.Contract.ClientId)
                .Select(group => new ClientSituation(group.ToList()))
                .OrderByDescending(s => s.Amount);

            foreach (var clientSituation in clientSituations)
            {
                var amount = GetAmount(cmrrSituation, clientSituation);
                if (amount is null)
                {
                    continue;
                }

                var topElement = CmrrAmountTopElement.FromRaw
                (
                    clientSituation.Amount,
                    clientSituation.ClientId,
                    CmrrAmountTopElementContract.FromRaw(0, clientSituation.ClientId, clientSituation.ClientName)
                );

                UpdateCmrrAmount(amount, clientSituation.Amount, clientSituation.UserCount, clientSituation.ClientId, null, topElement, hasTopLimit);
            }
        }

        private enum ClientSituationType
        {
            NotInteresting,
            Acquired,
            Terminated
        }

        private class ClientSituation
        {

            public ClientSituationType Type { get; }
            public string ClientName { get; }
            public decimal Amount { get; }
            public int ClientId { get; }
            public int ContractCount { get; }
            public int UserCount { get; }



            public ClientSituation(List<ContractAxisSectionSituation> situations)
            {
                Type = GetSituationType(situations);
                ClientName = situations.First().ContractSituation.Contract.ClientName;
                ClientId = situations.First().ContractSituation.Contract.ClientId;
                ContractCount = situations.Select(s => s.ContractSituation.ContractId).ToHashSet().Count;

                Amount = Type switch
                {
                    ClientSituationType.Acquired => situations.Sum(s => s.EndPeriodAmount),
                    ClientSituationType.Terminated => situations.Sum(s => s.StartPeriodAmount),
                    _ => Amount
                };

                UserCount = Type switch
                {
                    ClientSituationType.Acquired => situations.Max(s => s.EndPeriodUserCount),
                    ClientSituationType.Terminated => situations.Max(s => s.StartPeriodUserCount),
                    _ => UserCount
                };
            }

            private static ClientSituationType GetSituationType(List<ContractAxisSectionSituation> situations)
            {
                if (situations.All(c => c.ContractSituation.StartPeriodCount == null))
                {
                    return ClientSituationType.Acquired;
                }

                if (situations.All(c => c.ContractSituation.EndPeriodCount == null))
                {
                    return ClientSituationType.Terminated;
                }

                return ClientSituationType.NotInteresting;
            }
        }

        private CmrrSubLine GetSubLine(CmrrLine line, ContractAxisSectionSituation situation)
        {
            var subSectionName = situation.Breakdown.SubSection;
            line.SubLines.TryAdd(subSectionName, new CmrrSubLine(subSectionName));
            return line.SubLines[subSectionName];
        }

        private void UpdateLifecycleAmount(CmrrAmount amount, ContractAxisSectionSituation situation, bool hasTopLimit)
        {
            UpdateCmrrAmount(amount, situation, s => s.PartialDiff, s => s.UserCountDiff, _ => true, hasTopLimit);
        }

        private void UpdateStartAmount(CmrrAmount startAmount, ContractAxisSectionSituation situation, bool hasTopLimit)
        {
            UpdateCmrrAmount(startAmount, situation, s => s.StartPeriodAmount, s => s.StartPeriodUserCount, s => s.ContractSituation.StartPeriodCount != null, hasTopLimit);
        }

        private void UpdateEndAmount(CmrrAmount startAmount, ContractAxisSectionSituation situation, bool hasTopLimit)
        {
            UpdateCmrrAmount(startAmount, situation, s => s.EndPeriodAmount, s => s.EndPeriodUserCount, s => s.ContractSituation.EndPeriodCount != null, hasTopLimit);
        }

        private void UpdateCmrrAmount
        (
            CmrrAmount cmrrAmount,
            ContractAxisSectionSituation axisSectionSituation,
            Func<ContractAxisSectionSituation, decimal> amountFunc,
            Func<ContractAxisSectionSituation, int> userCountFunc,
            Func<ContractAxisSectionSituation, bool> shouldCountClientAndContracts,
            bool hasTopLimit
        )
        {
            var amount = amountFunc(axisSectionSituation);
            var userCount = userCountFunc(axisSectionSituation);
            var clientId = shouldCountClientAndContracts(axisSectionSituation)
                ? (int?)axisSectionSituation.ContractSituation.Contract.ClientId
                : null;
            var contractId = shouldCountClientAndContracts(axisSectionSituation)
                ? (int?)axisSectionSituation.ContractSituation.ContractId
                : null;

            UpdateCmrrAmount
            (
                cmrrAmount,
                amount,
                userCount,
                clientId,
                contractId,
                CmrrAmountTopElement.FromSituation(axisSectionSituation, amount, userCount),
                hasTopLimit
            );
        }

        private void UpdateCmrrAmount
        (
            CmrrAmount cmrrAmount,
            decimal amount,
            int userCount,
            int? clientId,
            int? contractId,
            CmrrAmountTopElement topElement,
            bool hasTopLimit
        )
        {
            cmrrAmount.Amount += amount;
            cmrrAmount.UserCount += userCount;

            if (clientId.HasValue)
            {
                cmrrAmount.AddClient(clientId.Value);
            }

            if (contractId.HasValue)
            {
                cmrrAmount.AddContract(contractId.Value);
            }

            if (!hasTopLimit || cmrrAmount.Top.Count < CmrrAmountTopElement.TopCount)
            {
                cmrrAmount.Top.Add(topElement);
            }
        }

        private CmrrAmount GetAmount(CmrrSituation cmrrSituation, ClientSituation clientSituation)
        {
            return clientSituation.Type switch
            {
                ClientSituationType.Acquired => cmrrSituation.Clients.Acquired,
                ClientSituationType.Terminated => cmrrSituation.Clients.Terminated,
                _ => null
            };
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
