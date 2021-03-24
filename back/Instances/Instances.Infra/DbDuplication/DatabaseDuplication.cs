using Distributors.Domain.Models;

namespace Instances.Infra.DbDuplication
{
    public enum DatabaseType
    {
        Production = 1,
        Training = 2,
        Preview = 3,
        Demos = 4
    }

    public class DatabaseDuplication
    {
        public DatabaseType Type { get; set; }
        public Distributor Distributor { get; set; }
        public string Cluster { get; set; }
    }
}
