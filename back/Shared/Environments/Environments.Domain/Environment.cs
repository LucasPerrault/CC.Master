using System.Collections.Generic;
using System.ComponentModel;

namespace Environments.Domain
{
    public class Environment
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public EnvironmentDomain Domain { get; set; }
        public EnvironmentPurpose Purpose { get; set; }
        public bool IsActive { get; set; }

        public string Host => $"https://{Subdomain}.{GetDomainAsString(Domain)}";

        public ICollection<EnvironmentSharedAccess> ActiveAccesses { get; set; }

        private static string GetDomainAsString(EnvironmentDomain domain)
        {
            return domain switch
            {
                EnvironmentDomain.ILuccaDotNet => "ilucca.net",
                EnvironmentDomain.ILuccaDotCh => "ilucca.ch",
                EnvironmentDomain.DauphineDotFr => "dauphine.fr",
                _ => throw new InvalidEnumArgumentException(nameof(domain), (int) domain, typeof(EnvironmentDomain))
            };
        }
    }

    public enum EnvironmentDomain
    {
        ILuccaDotNet = 0,
        ILuccaDotCh = 2,
        DauphineDotFr = 5
    }

    public enum EnvironmentPurpose
    {
        Contractual = 0,
        Lucca = 1,
        InternalUse = 2,
        QA = 3,
        Virgin = 4,
        Cluster = 5,
        Security = 6,
        InternalTest = 7,
        ExternalTest = 8,
        UrbaHack = 9
    }
}
