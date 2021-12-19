namespace Billing.Contracts.Domain.Offers
{
    public class ContractPricing
    {
        public int ContractId { get; set; }
        public PricingMethod PricingMethod { get; set; }
        public decimal ConstantPrice { get; set; }
        public decimal AnnualCommitmentPrice { get; set; }
        public decimal AnnualCommitmentUnits { get; set; }
    }
}
