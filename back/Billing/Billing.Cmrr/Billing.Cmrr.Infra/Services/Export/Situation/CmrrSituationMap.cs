using CsvHelper.Configuration;
using Resources.Translations;
using System;

namespace Billing.Cmrr.Infra.Services.Export.Situation
{
    internal class CmrrSituationMap : ClassMap<CmrrSituationCsvRow>
    {
        public CmrrSituationMap(CsvConfiguration config, DateTime from, DateTime to, ITranslations translations)
        {
            AutoMap(config);
            Map(s => s.Name).Name(translations.CmrrExportSituationAxis());
            Map(s => s.TotalFrom).Name(from.ToString("MMM yyyy"));
            Map(s => s.TotalTo).Name(to.ToString("MMM yyyy"));
            Map(s => s.Variation).Name(translations.CmrrExportSituationVariation());
            Map(s => s.VariationPercent).Name(string.Empty);
            Map(s => s.Creation).Name(translations.CmrrExportCreation());
            Map(s => s.Upsell).Name(translations.CmrrExportUpsell());
            Map(s => s.Expansion).Name(translations.CmrrExportExpansion());
            Map(s => s.Contraction).Name(translations.CmrrExportContraction());
            Map(s => s.Termination).Name(translations.CmrrExportTermination());
            Map(s => s.Nrr).Name(translations.CmrrExportSituationNrr());
            Map(s => s.ChurnRate).Name(translations.CmrrExportSituationChurn());
        }
    }
}
