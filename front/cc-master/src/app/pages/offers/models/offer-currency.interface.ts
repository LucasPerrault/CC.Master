import { currencyFields, ICurrency } from '@cc/domain/billing/offers';

export const offerCurrencyFields = `${ currencyFields },symbol,code`;

export interface IOfferCurrency extends ICurrency {
  symbol: string;
  code: number;
}

export enum CurrencyCode {
  EUR = 978,
  CHF = 756,
}

export const currencies: IOfferCurrency[] = [
  {
    code: CurrencyCode.EUR,
    name: 'EUR',
    symbol: '€',
  },
  {
    code: CurrencyCode.CHF,
    symbol: 'CHF',
    name: 'CHF',
  },
];

export const getCurrency = (code: CurrencyCode): IOfferCurrency =>
  currencies.find(c => c.code === code);
