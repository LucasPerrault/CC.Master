import { BillingMode } from '../enums/billing-mode.enum';
import { BillingUnit } from '../enums/billing-unit.enum';
import { IPriceListCreationDto } from './price-list-creation-dto.interface';

export interface IOfferCreationDto {
  billingMode: BillingMode;
  currencyID: number;
  forecastMethod: string;
  name: string;
  priceLists: IPriceListCreationDto[];
  pricingMethod: string;
  productId: number;
  sageBusiness: string;
  tag: string;
  unit: BillingUnit;
}
