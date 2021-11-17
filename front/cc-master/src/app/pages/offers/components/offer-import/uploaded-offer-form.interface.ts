import { IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../../enums/billing-mode.enum';
import { IBillingUnit } from '../../enums/billing-unit.enum';
import { IOfferCurrency } from '../../models/offer-currency.interface';
import { IPriceListForm } from '../../models/price-list-form.interface';

export interface IUploadedOfferForm {
  billingMode: IBillingMode;
  currency: IOfferCurrency;
  forecastMethod: string;
  name: string;
  priceLists: IPriceListForm[];
  pricingMethod: string;
  product: IProduct;
  category: string;
  billingUnit: IBillingUnit;
}
