using Instances.Domain.Renaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Instances.Infra.Iis
{
    public class RedirectionIisConfiguration
    {
        public string ApplicationHost { get; set; }
        public string RedirectionConf { get; set; }
    }

    public class RedirectionIisAdministration : IRedirectionIisAdministration
    {
        private readonly RedirectionIisConfiguration _configuration;

        public RedirectionIisAdministration(RedirectionIisConfiguration redirectionIisConfiguration)
        {
            _configuration = redirectionIisConfiguration;
        }

        public async Task BindDomainAsync(string newTenant, string domain)
        {
            XDocument doc;
            using (var stream = File.OpenRead(_configuration.ApplicationHost))
            {
                var ct = new CancellationToken();
                doc = await XDocument.LoadAsync(stream, LoadOptions.None, ct);
            }

            var httpBindingElement = new XElement("binding");
            httpBindingElement.SetAttributeValue("protocol", "http");
            httpBindingElement.SetAttributeValue("bindingInformation", $"*:80:{newTenant}.{domain}");

            var httpsBindingElement = new XElement("binding");
            httpsBindingElement.SetAttributeValue("protocol", "https");
            httpsBindingElement.SetAttributeValue("bindingInformation", $"*:443:{newTenant}.{domain}");
            httpsBindingElement.SetAttributeValue("sslFlags", "1");

            var bindingElement = doc
                .Element("configuration")
                .Element("system.applicationHost")
                .Element("sites")
                .Elements()
                .First(e => e.Attribute("name").Value == "redirect")
                .Element("bindings");
            bindingElement.Add(httpBindingElement);
            bindingElement.Add(httpsBindingElement);

            using (var writer = File.CreateText(_configuration.ApplicationHost))
            {
                var ct = new CancellationToken();
                await doc.SaveAsync(writer, SaveOptions.OmitDuplicateNamespaces, ct);
            }
        }

        public async Task CreateRedirectionAsync(string oldTenant, string newTenant, string domain, DateOnly endDate)
        {
            RedirectionConf redirectionConf;
            using (var stream = File.OpenRead(_configuration.RedirectionConf))
            {
                redirectionConf = await JsonSerializer.DeserializeAsync<RedirectionConf>(stream);
            }
            redirectionConf.Redirections.Insert(0, new RedirectionElementConf
            {
                TargetDomain = domain,
                SourceSubDomain = oldTenant,
                TargetSubDomain = newTenant,
                EndDate = endDate.ToString("dd/MM/yyyy")
            });

            var result = JsonSerializer.Serialize(redirectionConf, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_configuration.RedirectionConf, result);
        }

        private class RedirectionConf
        {
            [JsonPropertyName("redirections")]
            public List<RedirectionElementConf> Redirections { get; set; }
        }
        private class RedirectionElementConf
        {
            [JsonPropertyName("domainCible")]
            public string TargetDomain { get; set; }
            [JsonPropertyName("dateFin")]
            public string EndDate { get; set; }
            [JsonPropertyName("subdomainCible")]
            public string TargetSubDomain { get; set; }
            [JsonPropertyName("subdomainSource")]
            public string SourceSubDomain { get; set; }
        }
    }
}
