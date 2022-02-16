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
            var linesBySection = evolution.Lines
                .OrderBy(l => l.Section.Order)
                .ThenBy(l => l.Section.Id)
                .ThenBy(l => l is CmrrEvolutionBreakdownTotalLine ? 0 : 1)
                .GroupBy(l => l.SubSectionName)
                .Select(g => g.OrderBy(l => l.Period));

            var properties = new List<(string Name, Func<CmrrEvolutionBreakdownLine, decimal> Get)>
            {
                ( translations.CmrrExportEvolutionAmount(), l => l.Amount ),
                ( translations.CmrrExportCreation(), l => l.Creation ),
                ( translations.CmrrExportUpsell(), l => l.Upsell ),
                ( translations.CmrrExportExpansion(), l => l.Expansion ),
                ( translations.CmrrExportContraction(), l => l.Contraction ),
                ( translations.CmrrExportTermination(), l => l.Termination ),
            };

            var rows = new List<CmrrEvolutionCsvRow>();

            foreach (var sectionLines in linesBySection)
            {
                rows.AddRange(properties.Select(property =>
                    new CmrrEvolutionCsvRow
                    {
                        Name = sectionLines.First().SubSectionName,
                        AmountType = property.Name,
                        Amounts = sectionLines.Select(l => property.Get(l)).ToList()
                    }
                ));
            }

            return rows;
        }
    }
}
