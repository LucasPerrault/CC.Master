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

        private static string GetDomainAsString(EnvironmentDomain domain)
        {
            return domain switch
            {
                EnvironmentDomain.ILuccaDotNet => "ilucca.net",
                EnvironmentDomain.EpayeCpDotCom => "e-payecp.com",
                EnvironmentDomain.ILuccaDotCh => "ilucca.ch",
                EnvironmentDomain.MesCongesDotNet => "mesconges.net",
                EnvironmentDomain.UgoOnLineDotNet => "ugo-online.net",
                EnvironmentDomain.DauphineDotFr => "dauphine.fr",
                EnvironmentDomain.ILuccaPreviewDotNet => "ilucca-preview.net",
                EnvironmentDomain.FastobookDotCom => "fastobook.com",
                EnvironmentDomain.UrbaOnlineDotCom => "urbaonline.com",
                EnvironmentDomain.Local => "local.dev",
                EnvironmentDomain.Training => "ilucca-test.net",
                EnvironmentDomain.Demo => "ilucca-demo.net",
                _ => throw new InvalidEnumArgumentException(nameof(domain), (int) domain, typeof(EnvironmentDomain))
            };
        }
    }

    public enum EnvironmentDomain
    {
        ILuccaDotNet = 0,
        EpayeCpDotCom = 1,
        ILuccaDotCh = 2,
        MesCongesDotNet = 3,
        UgoOnLineDotNet = 4,
        DauphineDotFr = 5,
        ILuccaPreviewDotNet = 6,
        FastobookDotCom = 7,
        UrbaOnlineDotCom = 8,
        Local = 9,
        Training = 10,
        Demo = 11,
        Unknown = int.MaxValue,
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
