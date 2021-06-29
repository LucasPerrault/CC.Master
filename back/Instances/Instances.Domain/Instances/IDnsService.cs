using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{

    public enum DnsEntryZone
    {
        Demos = 1
    }

    public class DnsEntry
    {
        public string Subdomain { get; set; }
        public string Cluster { get; set; }
        public DnsEntryZone Zone { get; set; }

        private DnsEntry() { }

        public static DnsEntry ForDemo(string subdomain, string cluster) => new DnsEntry
        {
            Subdomain = subdomain,
            Cluster = cluster,
            Zone = DnsEntryZone.Demos
        };
    }

    public interface IDnsService
    {
        Task CreateAsync(DnsEntry entry);
        Task DeleteAsync(IEnumerable<DnsEntry> entries);
        Task DeleteAsync(DnsEntry entry);
    }
}
