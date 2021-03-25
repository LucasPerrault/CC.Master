namespace Instances.Domain.Demos
{
    // TODO ressource (db migration, store...)
    public class DemoDuplication
    {

        public string SourceDemoSubdomain { get; set; }

        public string Subdomain { get; set; }
        public bool IsStrictSubdomainSelection { get; set; }

        public string Comment { get; set; }

        public string Password { get; set; }
        public string DistributorId { get; set; }
    }
}
