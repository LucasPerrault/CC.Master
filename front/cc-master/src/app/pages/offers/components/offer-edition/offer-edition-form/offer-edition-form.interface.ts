import { IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../../../enums/billing-mode.enum';
import { IBillingUnit } from '../../../enums/billing-unit.enum';
import { IOfferCurrency } from '../../../models/offer-currency.interface';
import { IPriceListForm } from '../../../models/price-list-form.interface';

export interface IOfferEditionForm {
  name: string;
  product: IProduct;
  billingUnit: IBillingUnit;
  currency: IOfferCurrency;
  sageBusiness: string;
  tag: string;
  billingMode: IBillingMode;
  pricingMethod: string;
  forecastMethod: string;
  priceList: IPriceListForm;
}
