using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvancedFilters.Infra.Services
{
    public interface IExportService
    {
        FileStreamResult Export(List<Environment> environments, string filename);
        FileStreamResult Export(List<AppContact> contacts, string filename);
        FileStreamResult Export(List<ClientContact> contacts, string filename);
        FileStreamResult Export(List<SpecializedContact> contacts, string filename);
    }

    public class ExportCsvService : IExportService
    {
        public FileStreamResult Export(List<Environment> environments, string filename)
        {
            var csvEnvironments = environments.Select(e => new CsvEnvironment(e));

            return ExportAsync(csvEnvironments, filename);
        }

        public FileStreamResult Export(List<AppContact> contacts, string filename)
        {
            var csvContacts = contacts.Select(e => new CsvAppContact(e));

            return ExportAsync(csvContacts, filename);
        }

        public FileStreamResult Export(List<ClientContact> contacts, string filename)
        {
            var csvClientContacts = contacts.Select(e => new CsvClientContact(e));

            return ExportAsync(csvClientContacts, filename);
        }

        public FileStreamResult Export(List<SpecializedContact> contacts, string filename)
        {
            var csvSpecializedContacts = contacts.Select(e => new CsvSpecializedContact(e));

            return ExportAsync(csvSpecializedContacts, filename);
        }

        private FileStreamResult ExportAsync<T>(IEnumerable<T> csvData, string filename)
        {
            var configuration = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };

            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            using (var csvWriter = new CsvWriter(writer, configuration))
            {
                csvWriter.WriteRecords(csvData);

                writer.Flush();
                stream.Position = 0;
            }

            return new FileStreamResult(stream, "text/csv")
            {
                FileDownloadName = filename
            };
        }

        private class CsvEnvironment
        {
            public string EnvironmentName { get; set; }
            public string AppInstances { get; set; }
            public string LuCountries { get; set; }
            public string Distributors { get; set; }
            public string Cluster { get; set; }

            public System.DateTime CreatedAt { get; set; }

            public CsvEnvironment(Environment environment)
            {
                CreatedAt = environment.CreatedAt;
                Cluster = environment.Cluster;
                EnvironmentName = $"{environment.Subdomain}.{environment.Domain}";
                Distributors = string.Join(",", environment.Accesses.Select(a => a.Distributor.Name));
                AppInstances = string.Join(",", environment.AppInstances.Select(a => a.ApplicationName));
                LuCountries = string.Join(",", environment.LegalUnits.Select(x => x.Country.Name).Distinct());
            }

        }

        private class CsvAppContact
        {
            public string EnvironmentName { get; set; }
            public string UserLastName { get; set; }

            public string UserFirstName { get; set; }
            public string UserMail { get; set; }
            public string AppInstance { get; set; }
            public bool IsConfirmed { get; set; }

            public System.DateTime CreatedAt { get; set; }
            public System.DateTime? ExpiredAt { get; set; }

            public CsvAppContact(AppContact contact)
            {
                EnvironmentName = $"{contact.Environment.Subdomain}.{contact.Environment.Domain}";
                UserLastName = contact.UserLastName;
                UserFirstName = contact.UserFirstName;
                UserMail = contact.UserMail;
                AppInstance = contact.AppInstance.ApplicationName;
                IsConfirmed = contact.IsConfirmed;
                CreatedAt = contact.CreatedAt;
                ExpiredAt = contact.ExpiresAt;
            }

        }

        private class CsvClientContact
        {
            public string EnvironmentName { get; set; }
            public string UserLastName { get; set; }

            public string UserFirstName { get; set; }
            public string UserMail { get; set; }
            public string Client { get; set; }
            public bool IsConfirmed { get; set; }

            public System.DateTime CreatedAt { get; set; }
            public System.DateTime? ExpiredAt { get; set; }

            public CsvClientContact(ClientContact contact)
            {
                EnvironmentName = $"{contact.Environment.Subdomain}.{contact.Environment.Domain}";
                UserLastName = contact.UserLastName;
                UserFirstName = contact.UserFirstName;
                UserMail = contact.UserMail;
                Client = contact.Client.Name;
                IsConfirmed = contact.IsConfirmed;
                CreatedAt = contact.CreatedAt;
                ExpiredAt = contact.ExpiresAt;
            }
        }

        private class CsvSpecializedContact
        {
            public string EnvironmentName { get; set; }
            public string UserLastName { get; set; }

            public string UserFirstName { get; set; }
            public string UserMail { get; set; }
            public string Role { get; set; }
            public bool IsConfirmed { get; set; }

            public System.DateTime CreatedAt { get; set; }
            public System.DateTime? ExpiredAt { get; set; }

            public CsvSpecializedContact(SpecializedContact contact)
            {
                EnvironmentName = $"{contact.Environment.Subdomain}.{contact.Environment.Domain}";
                UserLastName = contact.UserLastName;
                UserFirstName = contact.UserFirstName;
                UserMail = contact.UserMail;
                Role = contact.RoleCode;
                IsConfirmed = contact.IsConfirmed;
                CreatedAt = contact.CreatedAt;
                ExpiredAt = contact.ExpiresAt;
            }
        }
    }
}
