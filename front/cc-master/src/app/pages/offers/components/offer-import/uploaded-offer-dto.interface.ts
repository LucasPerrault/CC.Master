import { IProduct } from '@cc/domain/billing/offers';

import { BillingMode } from '../../enums/billing-mode.enum';
import { BillingUnit } from '../../enums/billing-unit.enum';
import { CurrencyCode } from '../../models/offer-currency.interface';
import { IPriceListForm } from '../../models/price-list-form.interface';

export interface IUploadedOffer {
  billingMode: BillingMode;
  currencyID: CurrencyCode;
  forecastMethod: string;
  name: string;
  priceLists: IPriceListForm[];
  pricingMethod: string;
  product: IProduct;
  category: string;
  billingUnit: BillingUnit;
}
