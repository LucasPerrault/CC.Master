import { IPriceList } from '@cc/domain/billing/offers';

export interface IPriceListOfferSelectOption {
  id: number;
  name: string;
  priceList: IPriceList;
}
