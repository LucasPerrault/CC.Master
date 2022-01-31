using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Situation;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Evolution
{
    internal abstract class CmrrEvolutionLineBuilder<T>
    {
        private readonly IContractAxisSectionSituationsService _axisSectionSituationsService;

        public CmrrEvolutionLineBuilder(IContractAxisSectionSituationsService axisSectionSituationsService)
        {
            _axisSectionSituationsService = axisSectionSituationsService;
        }

        public abstract Task<IEnumerable<T>> GetEvolutionLinesAsync(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts, CmrrAxis axis, HashSet<string> selectedSections);

        protected Task<IEnumerable<ContractAxisSectionSituation>> GetAxisSectionSituations(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts, CmrrAxis axis)
        {
            var situations = GetEvolutionLineContractSituations(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts);
            return _axisSectionSituationsService.GetAxisSectionSituationsAsync(axis, situations);
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

        protected Func<decimal, Action<ICmrrEvolutionLine>> GetLifeCycleIncrementFunc(CmrrLifeCycle lifeCycle)
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
    }

    internal class CmrrEvolutionLineBuilder : CmrrEvolutionLineBuilder<CmrrEvolutionLine>
    {
        public CmrrEvolutionLineBuilder(IContractAxisSectionSituationsService axisSectionSituationsService)
            : base(axisSectionSituationsService)
        { }

        public override async Task<IEnumerable<CmrrEvolutionLine>> GetEvolutionLinesAsync(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts, CmrrAxis axis, HashSet<string> selectedSections)
        {
            var line = new CmrrEvolutionLine(currentCountPeriod);
            var hasSections = selectedSections.Any();

            foreach (var situation in await GetAxisSectionSituations(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts, axis))
            {
                if (hasSections && !selectedSections.Contains(situation.Breakdown.AxisSection.Name))
                {
                    continue;
                }
                line.Amount += situation.EndPeriodAmount;
                GetLifeCycleIncrementFunc(situation.ContractSituation.LifeCycle)(situation.PartialDiff)(line);
            }
            return new List<CmrrEvolutionLine> { line };
        }
    }

    internal class CmrrEvolutionBreakdownLineBuilder : CmrrEvolutionLineBuilder<CmrrEvolutionBreakdownLine>
    {
        private readonly ICmrrTranslations _translations;

        public CmrrEvolutionBreakdownLineBuilder(IContractAxisSectionSituationsService axisSectionSituationsService, ICmrrTranslations translations)
            : base(axisSectionSituationsService)
        {
            _translations = translations;
        }

        public override async Task<IEnumerable<CmrrEvolutionBreakdownLine>> GetEvolutionLinesAsync(DateTime currentCountPeriod, DateTime previousCountPeriod, Dictionary<CountKey, CmrrCount> countsByCountKey, List<CmrrContract> contracts, CmrrAxis axis, HashSet<string> selectedSections)
        {
            var hasSections = selectedSections.Any();

            var lines = new List<CmrrEvolutionBreakdownLine>();
            var allSituations = await GetAxisSectionSituations(currentCountPeriod, previousCountPeriod, countsByCountKey, contracts, axis);

            foreach (var situationsByAxis in allSituations.GroupBy(s => s.Breakdown.AxisSection))
            {
                if (hasSections && !selectedSections.Contains(situationsByAxis.Key.Name))
                {
                    continue;
                }

                var totalLine = new CmrrEvolutionBreakdownLine(currentCountPeriod)
                {
                    SectionName = _translations.CmrrExportTotalFormat(situationsByAxis.Key.Name),
                };
                lines.Add(totalLine);

                foreach (var situationsByBreakdown in situationsByAxis.GroupBy(s => s.Breakdown))
                {
                    var line = new CmrrEvolutionBreakdownLine(currentCountPeriod)
                    {
                        SectionName = situationsByBreakdown.Key.SubSection,
                    };
                    foreach (var situation in situationsByBreakdown)
                    {
                        totalLine.Amount += situation.EndPeriodAmount;
                        line.Amount += situation.EndPeriodAmount;
                        GetLifeCycleIncrementFunc(situation.ContractSituation.LifeCycle)(situation.PartialDiff)(line);
                        GetLifeCycleIncrementFunc(situation.ContractSituation.LifeCycle)(situation.PartialDiff)(totalLine);
                    }
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
