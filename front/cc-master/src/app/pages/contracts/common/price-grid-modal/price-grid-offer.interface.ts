import { IPriceList } from '@cc/domain/billing/offers';

export interface IPriceListOffer {
  currencyId: number;
  priceLists: IPriceList[];
}
