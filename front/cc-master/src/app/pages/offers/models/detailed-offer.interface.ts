import { IOffer, IPriceList } from '@cc/domain/billing/offers';

import { BillingMode } from '../enums/billing-mode.enum';
import { BillingUnit } from '../enums/billing-unit.enum';
import { ForecastMethod } from '../enums/forecast-method.enum';
import { PricingMethod } from '../enums/pricing-method.enum';
import { IOfferProduct } from './offer-product.interface';
import { IOfferUsage } from './offer-usage.interface';

export type IDetailedOfferWithoutUsage = Omit<IDetailedOffer, 'usage'>;

export interface IDetailedOffer extends IOffer {
  productId: number;
  product: IOfferProduct;
  billingMode: BillingMode;
  pricingMethod: PricingMethod;
  forecastMethod: ForecastMethod;
  tag: string;
  isCatalog: boolean;
  currencyId: number;
  isArchived: boolean;
  unit: BillingUnit;
  priceLists: IPriceList[];
  usage: IOfferUsage;
}
