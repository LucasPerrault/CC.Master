import { IPriceList, IProduct } from '@cc/domain/billing/offers';

import { BillingMode } from '../../enums/billing-mode.enum';
import { BillingUnit } from '../../enums/billing-unit.enum';
import { CurrencyCode } from '../../models/offer-currency.interface';

export interface IUploadedOffer {
  billingMode: BillingMode;
  currencyID: CurrencyCode;
  forecastMethod: string;
  name: string;
  priceLists: IPriceList[];
  pricingMethod: string;
  product: IProduct;
  category: string;
  billingUnit: BillingUnit;
}
