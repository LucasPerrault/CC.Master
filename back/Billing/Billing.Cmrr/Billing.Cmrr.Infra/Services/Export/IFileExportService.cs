using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Situation;
using Billing.Cmrr.Infra.Services.Export.Clients;
using Billing.Cmrr.Infra.Services.Export.Evolution;
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
        FileStreamResult Export(CmrrAxisEvolution evolution, string filename);
        FileStreamResult Export(IReadOnlyCollection<CmrrClient> clients, string filename);
    }

    public class CsvService : IFileExportService
    {
        private const string _delimiter = ";";

        private readonly ICmrrTranslations _translations;

        public CsvService(ICmrrTranslations translations)
        {
            _translations = translations;
        }

        public FileStreamResult Export(CmrrSituation situation, string filename)
        {
            var situationRows = situation.ToRows(_translations);

            return ExportAsync
            (
                situationRows,
                filename,
                config => new CmrrSituationMap(config, situation.StartPeriod, situation.EndPeriod, _translations)
            );
        }

        public FileStreamResult Export(CmrrAxisEvolution evolution, string filename)
        {
            var evolutionRows = evolution.ToRows(_translations);

            return ExportAsync
            (
                evolutionRows,
                filename,
                config => new CmrrEvolutionMap(config, evolution.Axis, evolution.Lines.Select(l => l.Period).Distinct().OrderBy(d => d), _delimiter, _translations)
            );
        }

        public FileStreamResult Export(IReadOnlyCollection<CmrrClient> clients, string filename)
        {
            var clientRows = clients.ToRows(_translations);

            return ExportAsync(clientRows, filename);
        }

        private FileStreamResult ExportAsync<T>(IEnumerable<T> csvData, string filename, Func<CsvConfiguration, ClassMap<T>> getMapFn = null)
        {
            var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                Delimiter = _delimiter,
                HasHeaderRecord = true,
                ShouldQuote = _ => false
            };
            var map = getMapFn?.Invoke(configuration);

            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var csvWriter = new CsvWriter(writer, configuration);

            if (map != null)
            {
                csvWriter.Context.RegisterClassMap(map);
            }

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