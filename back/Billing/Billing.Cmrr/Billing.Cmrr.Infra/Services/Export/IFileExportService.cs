using Billing.Cmrr.Domain.Situation;
using Billing.Cmrr.Infra.Services.Export.Situation;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Billing.Cmrr.Infra.Services.Export
{
    public interface IFileExportService
    {
        FileStreamResult Export(CmrrSituation situation, string filename);
    }

    public class CsvService : IFileExportService
    {
        private readonly ITranslations _translations;

        public CsvService(ITranslations translations)
        {
            _translations = translations;
        }

        public FileStreamResult Export(CmrrSituation situation, string filename)
        {
            var situationRows = new List<CmrrSituationCsvRow>();
            foreach (var line in situation.Lines)
            {
                situationRows.Add(new CmrrSituationCsvRow(line.Total, _translations.CmrrExportSituationTotalFormat(line.Name)));
                situationRows.AddRange(line.SubLines.Values.Select(sl => new CmrrSituationCsvRow(sl)));
            }
            situationRows.Add(new CmrrSituationCsvRow(situation.Total));

            return ExportAsync
            (
                situationRows,
                filename,
                config => new CmrrSituationMap(config, situation.StartPeriod, situation.EndPeriod, _translations)
            );
        }

        private FileStreamResult ExportAsync<T>(IEnumerable<T> csvData, string filename, Func<CsvConfiguration, ClassMap<T>> getMapFn)
        {
            var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };
            var map = getMapFn(configuration);

            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var csvWriter = new CsvWriter(writer, configuration);

            csvWriter.Context.RegisterClassMap(map);

            csvWriter.WriteRecords(csvData);

            writer.Flush();
            stream.Position = 0;

            return new FileStreamResult(stream, "text/csv")
            {
                FileDownloadName = filename
            };
        }
    }
}
