using AdvancedFilters.Domain.Billing.Models;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class EnvironmentAccess
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public int DistributorId { get; set; }
        public EnvironmentAccessType Type { get; set; }

        public Environment Environment { get; set; }
        public Distributor Distributor { get; set; }
    }

    public enum EnvironmentAccessType
    {
        Manual = 0,
        EnvironmentCreation = 1,
        Contract = 2,
        Subcontracting = 3,
    }
}
