using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface ICommercialOfferUsageService
    {
        Task<CommercialOfferUsage> BuildAsync(int offerId);
        Task<IReadOnlyCollection<CommercialOfferUsage>> BuildAsync(HashSet<int> offerId, AccessRight accessRight);
    }
}
