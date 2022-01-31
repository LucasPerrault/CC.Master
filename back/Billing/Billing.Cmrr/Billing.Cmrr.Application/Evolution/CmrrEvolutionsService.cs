using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Interfaces;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tools;

namespace Billing.Cmrr.Application.Evolution
{
    public class CmrrEvolutionsService : ICmrrEvolutionsService
    {
        private readonly ICmrrContractsStore _contractsStore;
        private readonly ICmrrCountsStore _countsStore;
        private readonly ICmrrRightsFilter _cmrrRightsFilter;
        private readonly IContractAxisSectionSituationsService _axisSectionSituationsService;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly ICmrrTranslations _translations;

        public CmrrEvolutionsService
        (
            ICmrrContractsStore contractsStore,
            ICmrrCountsStore countsStore,
            ICmrrRightsFilter cmrrRightsFilter,
            IContractAxisSectionSituationsService axisSectionSituationsService,
            ClaimsPrincipal claimsPrincipal,
            ICmrrTranslations translations
        )
        {
            _contractsStore = contractsStore;
            _countsStore = countsStore;
            _cmrrRightsFilter = cmrrRightsFilter;
            _axisSectionSituationsService = axisSectionSituationsService;
            _claimsPrincipal = claimsPrincipal;
            _translations = translations;
        }


        public async Task<CmrrEvolution> GetEvolutionAsync(CmrrFilter filter)
        {
            var lines = await GetLinesAsync(filter, new CmrrEvolutionLineBuilder(_axisSectionSituationsService)).ToListAsync();

            var evolution = new CmrrEvolution
            {
                Lines = lines,
                StartPeriod = filter.StartPeriod,
                EndPeriod = filter.EndPeriod
            };

            return evolution;
        }

        public async Task<CmrrAxisEvolution> GetAxisEvolutionAsync(CmrrFilter filter)
        {
            var lines = await GetLinesAsync(filter, new CmrrEvolutionBreakdownLineBuilder(_axisSectionSituationsService, _translations)).ToListAsync();

            var evolution = new CmrrAxisEvolution
            {
                Lines = lines,
                StartPeriod = filter.StartPeriod,
                EndPeriod = filter.EndPeriod,
                Axis = filter.Axis,
            };

            return evolution;
        }

        private async IAsyncEnumerable<T> GetLinesAsync<T>(CmrrFilter filter, CmrrEvolutionLineBuilder<T> lineBuilder)
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

                foreach (var line in await lineBuilder.GetEvolutionLinesAsync(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts, filter.Axis, filter.Sections))
                {
                    yield return line;
                }
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
    }

    internal class CountKey : ValueObject
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
