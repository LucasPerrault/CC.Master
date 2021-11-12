using System.Collections.Generic;

namespace Billing.Contracts.Domain.Offers
{
    public class CommercialOfferUsage
    {
        public int Id { get; set; }
        public List<CommercialOfferContract> Contracts { get; set; }
        public int NumberOfContracts { get; set; }
        public int NumberOfActiveContracts { get; set; }
        public int NumberOfNotStartedContracts { get; set; }
    }

    public class CommercialOfferContract
    {

    }
}
