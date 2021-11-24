import { IProduct } from '@cc/domain/billing/offers';

import { BillingMode } from '../../enums/billing-mode.enum';
import { BillingUnit } from '../../enums/billing-unit.enum';
import { ForecastMethod } from '../../enums/forecast-method.enum';
import { PricingMethod } from '../../enums/pricing-method.enum';
import { CurrencyCode } from '../../models/offer-currency.interface';
import { IPriceListForm } from '../../models/price-list-form.interface';

export interface IUploadedOffer {
  billingMode: BillingMode;
  currencyId: CurrencyCode;
  forecastMethod: ForecastMethod;
  name: string;
  priceLists: IPriceListForm[];
  pricingMethod: PricingMethod;
  product: IProduct;
  category: string;
  billingUnit: BillingUnit;
}
