import { IPriceList } from '@cc/domain/billing/offers';

export interface IOfferCreationDto {
  billingMode: number;
  currencyID: number;
  forecastMethod: string;
  name: string;
  priceLists: IPriceList[];
  pricingMethod: string;
  productId: number;
  sageBusiness: string;
  tag: string;
  unit: number;
}
