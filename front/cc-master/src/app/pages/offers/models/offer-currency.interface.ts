import { ICurrency } from '@cc/domain/billing/offers';

export interface IOfferCurrency extends ICurrency {
  symbol: string;
  code: number;
}

export enum CurrencyCode {
  EUR = 978,
  CHF = 756,
  CAD = 124,
}

export const currencies: IOfferCurrency[] = [
  {
    code: CurrencyCode.EUR,
    name: 'EUR',
    symbol: '€',
  },
  {
    code: CurrencyCode.CHF,
    name: 'CHF',
    symbol: 'CHF',
  },
  {
    code: CurrencyCode.CAD,
    name: 'CAD',
    symbol: '$C',
  },
];

export const getCurrency = (code: CurrencyCode): IOfferCurrency =>
  currencies.find(c => c.code === code);
