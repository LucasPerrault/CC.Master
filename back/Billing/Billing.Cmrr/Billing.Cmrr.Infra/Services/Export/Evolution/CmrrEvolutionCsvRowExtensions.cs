using Billing.Cmrr.Domain.Evolution;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Cmrr.Infra.Services.Export.Evolution
{
    internal static class CmrrEvolutionCsvRowExtensions
    {
        public static IEnumerable<CmrrEvolutionCsvRow> ToRows(this CmrrAxisEvolution evolution, ICmrrTranslations translations)
        {
            var linesBySubSection = evolution.Lines
                .OrderBy(l => l.Section.Order)
                .ThenBy(l => l.Section.Id)
                .ThenBy(l => l is CmrrEvolutionBreakdownTotalLine ? 0 : 1)
                .GroupBy(l => l.SubSectionName);

            var properties = new List<(string Name, Func<IEnumerable<CmrrEvolutionBreakdownLine>, decimal> Get)>
            {
                ( translations.CmrrExportEvolutionAmount(), ls => ls.Sum(s => s.Amount) ),
                ( translations.CmrrExportCreation(), ls => ls.Sum(s => s.Creation )),
                ( translations.CmrrExportUpsell(), ls => ls.Sum(s => s.Upsell )),
                ( translations.CmrrExportExpansion(), ls => ls.Sum(s => s.Expansion )),
                ( translations.CmrrExportContraction(), ls => ls.Sum(s => s.Contraction )),
                ( translations.CmrrExportTermination(), ls => ls.Sum(s => s.Termination )),
            };

            var rows = new List<CmrrEvolutionCsvRow>();

            foreach (var subSectionLines in linesBySubSection)
            {
                var linesByPeriod = subSectionLines.GroupBy(l => l.Period).ToDictionary(g => g.Key, g => g.ToList());
                var allPeriods = GetAllPeriods(evolution.StartPeriod, evolution.EndPeriod);

                var allLinesByPeriod = allPeriods.Select(p => linesByPeriod.ContainsKey(p) ? linesByPeriod[p] : new List<CmrrEvolutionBreakdownLine>());

                rows.AddRange(properties.Select(property =>
                    new CmrrEvolutionCsvRow
                    {
                        Name = subSectionLines.First().SubSectionName,
                        AmountType = property.Name,
                        Amounts = allLinesByPeriod.Select(g => property.Get(g)).ToList()
                    }
                ));
            }

            return rows;
        }

        private static IEnumerable<DateTime> GetAllPeriods(DateTime from, DateTime to)
        {
            var diffMonths = ((to.Year - from.Year) * 12) + to.Month - from.Month;
            return Enumerable
                    .Range(0, diffMonths + 1)
                    .Select(d => from.AddMonths(d));
        }
    }
}
