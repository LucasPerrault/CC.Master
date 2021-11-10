using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Core.Models;
using Tools;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class EnvironmentAccess : IDeepCopyable<EnvironmentAccess>
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public int DistributorId { get; set; }
        public EnvironmentAccessType Type { get; set; }

        public Environment Environment { get; set; }
        public Distributor Distributor { get; set; }

        public EnvironmentAccess DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }

    public enum EnvironmentAccessType
    {
        Manual = 0,
        EnvironmentCreation = 1,
        Contract = 2,
    }
}
