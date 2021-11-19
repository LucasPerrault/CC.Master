namespace Billing.Contracts.Domain.Offers
{
    public class CommercialOfferUsage
    {
        public int OfferId { get; set; }
        public int NumberOfContracts { get; set; }
        public int NumberOfActiveContracts { get; set; }
        public int NumberOfNotStartedContracts { get; set; }
        public int NumberOfCountedContracts { get; set; }
    }
}
