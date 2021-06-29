namespace Instances.Infra.Dns
{
    internal class DnsEntryCreation
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
        public string Cluster { get; set; }
    }

    internal class DnsEntryDeletion
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
    }
}
