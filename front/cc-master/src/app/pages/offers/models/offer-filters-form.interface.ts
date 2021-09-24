import { IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../enums/billing-mode.enum';
import { IOfferCurrency } from './offer-currency.interface';

export interface IOfferFiltersForm {
  search: string;
  tag: string;
  product: IProduct;
  currencies: IOfferCurrency[];
  billingModes: IBillingMode[];
}
