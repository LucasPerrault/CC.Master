import { currencyFields, ICurrency } from '@cc/domain/billing/offers';

export const offerCurrencyFields = `${ currencyFields },symbol,code`;

export interface IOfferCurrency extends ICurrency {
  symbol: string;
  code: number;
}
