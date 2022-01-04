using Instances.Domain.Instances;

namespace Instances.Infra.Dns
{
    public interface IDnsEntry
    {
        public string Subdomain { get; }
        public string DnsZone { get; }
    }

    public class DnsEntryCreation : IDnsEntry
    {
        public string Subdomain { get; init; }
        public DnsEntryZone DnsEntryZone { get; init; }
        public string DnsZone { get; init; }
        public string Cluster { get; init; }
    }

    public class DnsEntryDeletion : IDnsEntry
    {
        public string Subdomain { get; init; }
        public string DnsZone { get; init; }
    }
}
