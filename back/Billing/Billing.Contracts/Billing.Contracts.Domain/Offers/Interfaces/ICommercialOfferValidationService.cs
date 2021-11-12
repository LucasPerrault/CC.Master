namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface ICommercialOfferValidationService
    {
        void ThrowIfCannotCreateOffer(CommercialOffer newOffer);
        void ThrowIfCannotModifyOffer(CommercialOffer oldOffer, CommercialOffer newOffer, CommercialOfferUsage oldUsage);
        void ThrowIfCannotDeleteOffer(CommercialOffer offer, CommercialOfferUsage usage);
        void ThrowIfCannotAddPriceList(CommercialOffer offer, PriceList priceList);
        void ThrowIfCannotModifyPriceList(CommercialOffer offer, PriceList oldPriceList, PriceList newPriceList, CommercialOfferUsage oldUsage);
        void ThrowIfCannotDeletePriceList(CommercialOffer offer, PriceList priceList);
    }
}
