using Distributors.Domain.Models;

namespace Instances.Infra.DataDuplication
{
    public enum DatabaseType
    {
        Production = 1,
        Training = 2,
        Preview = 3,
        Demos = 4
    }

    public class TenantDataDuplication
    {
        public DatabaseType Type { get; set; }
        public Distributor Distributor { get; set; }
        public TenantDataSource Source { get; set; }
        public TenantDataSource Target { get; set; }
        public int AuthorId { get; set; }
    }

    public class TenantDataSource
    {
        public string ClusterName { get; set; }
        public string Subdomain { get; set; }
    }
}
