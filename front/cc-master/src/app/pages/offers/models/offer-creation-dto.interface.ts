import { IPriceListCreationDto } from './price-list-creation-dto.interface';

export interface IOfferCreationDto {
  billingMode: number;
  currencyID: number;
  forecastMethod: string;
  name: string;
  priceLists: IPriceListCreationDto[];
  pricingMethod: string;
  productId: number;
  sageBusiness: string;
  tag: string;
  unit: number;
}
