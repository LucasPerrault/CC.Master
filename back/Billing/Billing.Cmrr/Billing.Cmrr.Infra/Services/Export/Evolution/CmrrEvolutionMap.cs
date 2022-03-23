using Billing.Cmrr.Domain.Situation;
using CsvHelper.Configuration;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Billing.Cmrr.Infra.Services.Export.Evolution
{
    internal class CmrrEvolutionMap : ClassMap<CmrrEvolutionCsvRow>
    {
        private readonly ICmrrTranslations _translations;

        public CmrrEvolutionMap(CsvConfiguration config, CmrrAxis axis, IEnumerable<DateTime> periods, string delimiter, ICmrrTranslations translations)
        {
            _translations = translations;
            AutoMap(config);
            Map(s => s.Name).Name(GetAxisName(axis));
            Map(s => s.AmountType).Name(translations.CmrrExportEvolutionAmountType());
            Map(s => s.Amounts).Name(GetPeriodsColumnName(periods, delimiter));
        }

        private string GetAxisName(CmrrAxis axis)
            => axis switch
            {
                CmrrAxis.Product => _translations.CmrrExportEvolutionProduct(),
                CmrrAxis.BusinessUnit => _translations.CmrrExportEvolutionBu(),
                CmrrAxis.Solution => _translations.CmrrExportEvolutionSolution(),
                _ => throw new InvalidEnumArgumentException(nameof(axis), (int)axis, typeof(CmrrAxis)),
            };

        private static string GetPeriodsColumnName(IEnumerable<DateTime> periods, string delimiter)
        {
            return string.Join(delimiter, periods.Select(p => p.ToString("MMM yyyy")));
        }
    }
}
