import { currencyFields, ICurrency } from './currency.interface';

export const offerPriceListFields = `currency[${currencyFields}],priceLists[id,rows[id,maxIncludedCount,unitPrice,fixedPrice,listId]]`;
export interface IPriceListOffer {
  currency: ICurrency;
  priceLists: IPriceList[];
}

export interface IPriceList {
  id: number;
  rows: IPriceRow[];
}

export interface IPriceRow {
  id: number;
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
  listId: number;
}
