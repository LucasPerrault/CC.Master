using AdvancedFilters.Domain.Instance.Models;

namespace AdvancedFilters.Domain.Billing.Models
{
    public class EstablishmentContract
    {
        public int ContractId { get; set; }
        public int EstablishmentId { get; set; }
        public int EnvironmentId { get; set; }

        public Contract Contract { get; set; }
        public Establishment Establishment { get; set; }
    }
}
