using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvancedFilters.Infra.Services
{
    public interface IExportService
    {
        Stream ExportAsync(List<Environment> environments);
        Stream ExportAsync(List<AppContact> contacts);
        Stream ExportAsync(List<ClientContact> contacts);
        Stream ExportAsync(List<SpecializedContact> contacts);

    }

    public class ExportCsvService : IExportService
    {
        public Stream ExportAsync(List<Environment> environments)
        {
            var csvEnvironments = environments.Select(e => new CsvEnvironment(e));

            return ExportAsync(csvEnvironments);
        }

        public Stream ExportAsync(List<AppContact> contacts)
        {
            var csvContacts = contacts.Select(e => new CsvAppContact(e));

            return ExportAsync(csvContacts);
        }

        public Stream ExportAsync(List<ClientContact> contacts)
        {
            var csvEnvironments = contacts.Select(e => new CsvClientContact(e));

            return ExportAsync(csvEnvironments);
        }

        public Stream ExportAsync(List<SpecializedContact> contacts)
        {
            var csvEnvironments = contacts.Select(e => new CsvSpecializedContact(e));

            return ExportAsync(csvEnvironments);
        }

        private Stream ExportAsync<T>(IEnumerable<T> csvData)
        {
            var configuration = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };

            var mem = new MemoryStream();
            using (var writer = new StreamWriter(mem, leaveOpen: true))
            using (var csvWriter = new CsvWriter(writer, configuration))
            {
                csvWriter.WriteRecords(csvData);

                writer.Flush();
                mem.Position = 0;
            }

            return mem;
        }

        private class CsvEnvironment
        {
            public string EnvironmentName { get; set; }
            public string AppInstances { get; set; }

            public string LuCountries { get; set; }
            public string Cluster { get; set; }

            public System.DateTime CreatedAt { get; set; }

            public CsvEnvironment(Environment environment)
            {
                CreatedAt = environment.CreatedAt;
                Cluster = environment.Cluster;
                EnvironmentName = $"{environment.Subdomain}.{environment.Domain}";
                AppInstances = string.Join(",", environment.AppInstances.Select(a => a.Name));
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
                AppInstance = contact.AppInstance.Name;
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
