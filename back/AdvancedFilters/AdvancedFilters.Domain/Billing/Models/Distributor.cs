using AdvancedFilters.Domain.Core.Models;
using Tools;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class Distributor : IDeepCopyable<Distributor>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public bool IsAllowingCommercialCommunication { get; set; }

        // TODO Handle loop breaking once distributors have EnvironmentAccesses

        public Distributor DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
