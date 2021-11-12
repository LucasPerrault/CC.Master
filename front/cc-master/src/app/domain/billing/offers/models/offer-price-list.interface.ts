import { currencyFields, ICurrency } from './currency.interface';
import { IPriceRow, priceRowFields } from './offer-price-row.interface';

export const priceListFields = `id,startsOn,rows[${ priceRowFields }]`;
export interface IPriceList {
  id: number;
  startsOn: string;
  rows: IPriceRow[];
}

export const offerPriceListFields = `currency[${currencyFields}],priceLists[${ priceListFields }]`;
export interface IPriceListOffer {
  currency: ICurrency;
  priceLists: IPriceList[];
}
