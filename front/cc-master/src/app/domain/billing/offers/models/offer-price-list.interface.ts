import { currencyFields, ICurrency } from './currency.interface';

export const offerPriceListFields = [
  `currency[${currencyFields}]`,
  'priceLists[id,startsOn,rows[id,maxIncludedCount,unitPrice,fixedPrice,listId]]',
].join(',');

export interface IPriceListOffer {
  currency: ICurrency;
  priceLists: IPriceList[];
}

export interface IPriceList {
  id: number;
  rows: IPriceRow[];
  startsOn: string;
}

export interface IPriceRow {
  id: number;
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
  listId: number;
}

export const offerPriceListFields = `currency[${currencyFields}],priceLists[${ priceListFields }]`;
export interface IOfferPriceList {
  currency: ICurrency;
  priceLists: IPriceList[];
}
