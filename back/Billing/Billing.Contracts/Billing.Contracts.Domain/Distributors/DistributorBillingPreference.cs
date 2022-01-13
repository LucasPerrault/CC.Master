using Distributors.Domain.Models;

namespace Billing.Contracts.Domain.Distributors;

public class DistributorBillingPreference
{
    public int DistributorId { get; set; }
    public Distributor Distributor { get; set; }

    public bool IsEnforcingMinimalBilling { get; set; }
}
