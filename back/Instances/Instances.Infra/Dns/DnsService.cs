using Instances.Domain.Instances;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Instances.Infra.Dns
{
    public class DnsZonesConfiguration
    {
        public string Demos { get; set; }
    }

    public class DnsService : IDnsService
    {
        private readonly InternalDnsService _internalDnsService;
        private readonly DnsZonesConfiguration _dnsZonesConfiguration;

        public DnsService(InternalDnsService internalDnsService, DnsZonesConfiguration dnsZonesConfiguration)
        {
            _internalDnsService = internalDnsService;
            _dnsZonesConfiguration = dnsZonesConfiguration;
        }

        public Task CreateAsync(DnsEntry entry)
        {
            _internalDnsService.AddNewCname(GetDomain(entry), entry.Cluster);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<DnsEntry> entries)
        {
            foreach (var entry in entries)
            {
                _internalDnsService.DeleteCname(entry.Subdomain);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DnsEntry entry)
        {
            _internalDnsService.DeleteCname(entry.Subdomain);
            return Task.CompletedTask;
        }

        private string GetDomain(DnsEntry entry)
        {
            return entry.Zone switch
            {
                DnsEntryZone.Demos => $"{entry.Subdomain}.{_dnsZonesConfiguration.Demos}",
                _ => throw new InvalidEnumArgumentException(nameof(entry.Zone), (int)entry.Zone, typeof(DnsEntryZone))
            };
        }
    }
}
