using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface ICommercialOfferUsageService
    {
        Task<CommercialOfferUsage> BuildAsync(int offerId);
    }
}
