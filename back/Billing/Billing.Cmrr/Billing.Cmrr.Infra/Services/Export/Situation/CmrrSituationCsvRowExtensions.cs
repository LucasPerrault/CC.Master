using Billing.Cmrr.Domain.Situation;
using Resources.Translations;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Cmrr.Infra.Services.Export.Situation
{
    internal static class CmrrSituationCsvRowExtensions
    {
        public static IEnumerable<CmrrSituationCsvRow> ToRows(this CmrrSituation situation, ICmrrTranslations translations)
        {
            var situationRows = new List<CmrrSituationCsvRow>();

            foreach (var line in situation.Lines)
            {
                situationRows.Add(new CmrrSituationCsvRow(line.Total, translations.CmrrExportTotalFormat(line.Name)));
                situationRows.AddRange(line.SubLines.Values.Select(sl => new CmrrSituationCsvRow(sl)));
            }
            situationRows.Add(new CmrrSituationCsvRow(situation.Total));

            return situationRows;
        }
    }
}
