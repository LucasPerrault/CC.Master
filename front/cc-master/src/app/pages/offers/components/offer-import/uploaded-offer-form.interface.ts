import { IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../../enums/billing-mode.enum';
import { IBillingUnit } from '../../enums/billing-unit.enum';
import { ForecastMethod } from '../../enums/forecast-method.enum';
import { PricingMethod } from '../../enums/pricing-method.enum';
import { IOfferCurrency } from '../../models/offer-currency.interface';
import { IPriceListForm } from '../../models/price-list-form.interface';

export interface IUploadedOfferForm {
  billingMode: IBillingMode;
  currency: IOfferCurrency;
  forecastMethod: ForecastMethod;
  name: string;
  priceLists: IPriceListForm[];
  pricingMethod: PricingMethod;
  product: IProduct;
  category: string;
  billingUnit: IBillingUnit;
}
