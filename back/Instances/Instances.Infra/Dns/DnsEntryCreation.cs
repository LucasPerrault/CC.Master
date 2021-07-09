namespace Instances.Infra.Dns
{
    public interface IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
    }

    public class DnsEntryCreation : IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
        public string Cluster { get; set; }
    }

    public class DnsEntryDeletion : IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
    }
}
