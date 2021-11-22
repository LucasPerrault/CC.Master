import { IProduct } from '@cc/domain/billing/offers';

import { OfferState } from '../components/offer-filters/offer-state-filter';
import { IBillingMode } from '../enums/billing-mode.enum';
import { IOfferCurrency } from './offer-currency.interface';

export interface IOfferFiltersForm {
  search: string;
  tag: string;
  product: IProduct;
  currencies: IOfferCurrency[];
  billingModes: IBillingMode[];
  state: OfferState;
}

export enum OfferFilterKey {
  Search = 'search',
  Tag = 'tag',
  Product = 'product',
  Currencies = 'currencies',
  BillingModes = 'billingModes',
  State = 'state',
}
