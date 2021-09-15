import { currencyFields, ICurrency } from './currency.interface';

export const offerPriceListFields = `currency[${currencyFields}],priceLists[id,lowerBound,upperBound,unitPrice,fixedPrice]`;
export interface IOfferPriceList {
  currency: ICurrency;
  priceLists: IPriceList[];
}

export interface IPriceList {
  id: number;
  lowerBound: number;
  upperBound: number;
  unitPrice: number;
  fixedPrice: number;
}
