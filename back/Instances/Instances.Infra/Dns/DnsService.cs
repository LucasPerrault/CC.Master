using Instances.Domain.Instances;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Instances.Infra.Dns
{
    public class DnsConfiguration
    {
        public DnsZonesConfiguration Zones { get; set; }
        public InternalDnsConfiguration Internal { get; set; }
        public OvhDnsConfiguration Ovh { get; set; }
    }

    public class DnsZonesConfiguration
    {
        public string Demos { get; set; }
    }

    public class DnsService : IDnsService
    {
        private readonly InternalDnsService _internalDnsService;
        private readonly OvhDnsService _ovhDnsService;
        private readonly DnsZonesConfiguration _dnsZones;

        public DnsService(InternalDnsService internalDnsService, OvhDnsService ovhDnsService, DnsZonesConfiguration dnsZones)
        {
            _internalDnsService = internalDnsService;
            _ovhDnsService = ovhDnsService;
            _dnsZones = dnsZones;
        }

        public async Task CreateAsync(DnsEntry entry)
        {
            // On nettoie pour être sûr
            await DeleteAsync(entry);

            var creation = new DnsEntryCreation
            {
                Subdomain = entry.Subdomain,
                DnsZone = GetDnsZoneAsString(entry),
                Cluster = entry.Cluster,
            };

            _internalDnsService.AddNewCname(creation);
            await _ovhDnsService.AddNewCnameAsync(creation);
        }

        public async Task DeleteAsync(IEnumerable<DnsEntry> entries)
        {
            foreach (var entry in entries)
            {
                var deletion = new DnsEntryDeletion { Subdomain = entry.Subdomain, DnsZone = GetDnsZoneAsString(entry) };
                _internalDnsService.DeleteCname(deletion);
                await _ovhDnsService.DeleteCnameAsync(deletion);
            }
        }

        public async Task DeleteAsync(DnsEntry entry)
        {
            var deletion = new DnsEntryDeletion { Subdomain = entry.Subdomain, DnsZone = GetDnsZoneAsString(entry) };
            _internalDnsService.DeleteCname(deletion);
            await _ovhDnsService.DeleteCnameAsync(deletion);
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
