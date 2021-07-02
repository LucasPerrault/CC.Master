namespace Instances.Infra.Dns
{
    internal interface IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
    }

    internal class DnsEntryCreation : IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
        public string Cluster { get; set; }
    }

    internal class DnsEntryDeletion : IDnsEntry
    {
        public string Subdomain { get; set; }
        public string DnsZone { get; set; }
    }
}
