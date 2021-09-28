import { IPriceList, IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../../enums/billing-mode.enum';
import { IBillingUnit } from '../../enums/billing-unit.enum';
import { IOfferCurrency } from '../../models/offer-currency.interface';

export interface IOfferForm {
  name: string;
  product: IProduct;
  billingUnit: IBillingUnit;
  currency: IOfferCurrency;
  sageBusiness: string;
  tag: string;
  billingMode: IBillingMode;
  pricingMethod: string;
  forecastMethod: string;
  priceLists: IPriceList[];
}
