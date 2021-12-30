using Environments.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{

    public enum DnsEntryZone
    {
        Demos = 1,
        RbxProductions = 2,
        ChProductions = 3,
        Previews = 4,
        Trainings = 5
    }

    public class DnsEntry
    {
        public string Subdomain { get; private init; }
        public string Cluster { get; private init; }
        public DnsEntryZone Zone { get; private init; }

        private DnsEntry() { }

        public static DnsEntry ForDemo(string subdomain, string cluster) => new DnsEntry
        {
            Subdomain = subdomain,
            Cluster = cluster,
            Zone = DnsEntryZone.Demos
        };

        public static DnsEntry ForPreview(string subdomain, string cluster) => new DnsEntry
        {
            Subdomain = subdomain,
            Cluster = cluster,
            Zone = DnsEntryZone.Previews
        };

        public static DnsEntry ForProduction(string subdomain, string cluster, EnvironmentDomain domain) => new DnsEntry
        {
            Subdomain = subdomain,
            Cluster = cluster,
            Zone = domain switch
            {
                EnvironmentDomain.ILuccaDotNet => DnsEntryZone.RbxProductions,
                EnvironmentDomain.ILuccaDotCh => DnsEntryZone.ChProductions,
                _ => throw new NotSupportedException($"Renaming of domain {domain} is not supported")
            }
        };

        public static DnsEntry ForTraining(string subdomain, string cluster) => new DnsEntry
        {
            Subdomain = subdomain,
            Cluster = cluster,
            Zone = DnsEntryZone.Trainings
        };

    }

    public interface IDnsService
    {
        Task CreateAsync(DnsEntry entry);
        Task DeleteAsync(IEnumerable<DnsEntry> entries);
        Task DeleteAsync(DnsEntry entry);
    }
}
