import { BillingMode } from '../enums/billing-mode.enum';
import { BillingUnit } from '../enums/billing-unit.enum';

export interface IOfferEditionDto {
  id: number;
  name: string;
  productId: number;
  pricingMethod: string;
  billingMode: BillingMode;
  unit: BillingUnit;
  tag: string;
  currencyID: number;
  forecastMethod: string;
  isArchived: boolean;
}
