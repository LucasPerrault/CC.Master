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
        private readonly DnsZonesConfiguration _dnsZones;

        public DnsService(InternalDnsService internalDnsService, DnsZonesConfiguration dnsZones)
        {
            _internalDnsService = internalDnsService;
            _dnsZones = dnsZones;
        }

        public Task CreateAsync(DnsEntry entry)
        {
            var creation = new DnsEntryCreation
            {
                Subdomain = entry.Subdomain,
                DnsZone = GetDnsZoneAsString(entry),
                Cluster = entry.Cluster,
            };

            _internalDnsService.AddNewCname(creation);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<DnsEntry> entries)
        {
            foreach (var entry in entries)
            {
                var deletion = new DnsEntryDeletion { Subdomain = entry.Subdomain, DnsZone = GetDnsZoneAsString(entry) };
                _internalDnsService.DeleteCname(deletion);
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(DnsEntry entry)
        {
            var deletion = new DnsEntryDeletion { Subdomain = entry.Subdomain, DnsZone = GetDnsZoneAsString(entry) };
            _internalDnsService.DeleteCname(deletion);
            return Task.CompletedTask;
        }

        private string GetDnsZoneAsString(DnsEntry entry)
        {
            return entry.Zone switch
            {
                DnsEntryZone.Demos => _dnsZones.Demos,
                _ => throw new InvalidEnumArgumentException(nameof(entry.Zone), (int)entry.Zone, typeof(DnsEntryZone))
            };
        }
    }
}
