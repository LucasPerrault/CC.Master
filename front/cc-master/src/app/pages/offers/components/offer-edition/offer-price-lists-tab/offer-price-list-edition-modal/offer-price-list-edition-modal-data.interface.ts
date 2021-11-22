import { IPriceList } from '@cc/domain/billing/offers';

import { IDetailedOffer } from '../../../../models/detailed-offer.interface';

export interface IOfferPriceListEditionModalData {
  offer: IDetailedOffer;
  priceListToEdit: IPriceList;
  allListStartDates: Date[];
}
