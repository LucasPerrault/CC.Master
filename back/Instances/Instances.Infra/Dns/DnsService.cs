using Instances.Domain.Instances;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Instances.Infra.Dns
{
    public class DnsConfiguration
    {
        public DnsZonesConfiguration Zones { get; set; }
        public WinDnsConfiguration Internal { get; set; }
        public OvhDnsConfiguration Ovh { get; set; }
    }

    public class DnsZonesConfiguration
    {
        public string Demos { get; set; }
        public string RbxProductions { get; set; }
        public string ChProductions { get; set; }
        public string Previews { get; set; }
        public string Trainings { get; set; }
    }


    public interface IInternalDnsService
    {
        void AddNewCname(DnsEntryCreation entryCreation);
        void DeleteCname(DnsEntryDeletion entryDeletion);
    }

    public interface IExternalDnsService
    {
        Task AddNewCnameAsync(DnsEntryCreation entryCreation);
        Task DeleteCnameAsync(DnsEntryDeletion entryDeletion);
    }

    public class DnsService : IDnsService
    {
        private readonly IInternalDnsService _internalDnsService;
        private readonly IExternalDnsService _externalDnsService;
        private readonly DnsZonesConfiguration _dnsZones;

        public DnsService(IInternalDnsService internalDnsService, IExternalDnsService externalDnsService, DnsZonesConfiguration dnsZones)
        {
            _internalDnsService = internalDnsService;
            _externalDnsService = externalDnsService;
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
            await _externalDnsService.AddNewCnameAsync(creation);
        }

        public async Task DeleteAsync(IEnumerable<DnsEntry> entries)
        {
            foreach (var entry in entries)
            {
                await DeleteAsync(entry);
            }
        }

        public async Task DeleteAsync(DnsEntry entry)
        {
            var deletion = new DnsEntryDeletion { Subdomain = entry.Subdomain, DnsZone = GetDnsZoneAsString(entry) };
            _internalDnsService.DeleteCname(deletion);
            await _externalDnsService.DeleteCnameAsync(deletion);
        }

        private string GetDnsZoneAsString(DnsEntry entry)
        {
            return entry.Zone switch
            {
                DnsEntryZone.Demos => _dnsZones.Demos,
                DnsEntryZone.RbxProductions => _dnsZones.RbxProductions,
                DnsEntryZone.ChProductions => _dnsZones.ChProductions,
                DnsEntryZone.Previews => _dnsZones.Previews,
                DnsEntryZone.Trainings => _dnsZones.Trainings,
                _ => throw new InvalidEnumArgumentException(nameof(entry.Zone), (int)entry.Zone, typeof(DnsEntryZone))
            };
        }
    }
}
