import { IOffer, IPriceList, offerFields, priceListFields } from '@cc/domain/billing/offers';

export const priceListsOfferFields = `${ offerFields },priceLists[${ priceListFields }]`;

export interface IPriceListsOffer extends IOffer {
  priceLists: IPriceList[];
}
