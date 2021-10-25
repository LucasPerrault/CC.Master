using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Models;
using Tools;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class EstablishmentContract : IDeepCopyable<EstablishmentContract>
    {
        public int ContractId { get; set; }
        public int EstablishmentId { get; set; }
        public int EnvironmentId { get; set; }

        public Contract Contract { get; set; }
        public Establishment Establishment { get; set; }

        public EstablishmentContract DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
