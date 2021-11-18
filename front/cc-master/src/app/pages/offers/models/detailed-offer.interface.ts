import { IOffer, IPriceList, offerFields, priceListFields } from '@cc/domain/billing/offers';

import { BillingMode } from '../enums/billing-mode.enum';
import { BillingUnit } from '../enums/billing-unit.enum';
import { IOfferCurrency, offerCurrencyFields } from './offer-currency.interface';
import { IOfferProduct, offerProductFields } from './offer-product.interface';

export const detailedOfferFields = [
  'collection.count',
  offerFields,
  'tag',
  'billingMode',
  'currencyID',
  'isCatalog',
  'pricingMethod',
  'unit',
  'productId',
  'forecastMethod',
  'activeContractNumber',
  `currency[${ offerCurrencyFields }]`,
  `product[${ offerProductFields }]`,
  `priceLists[${ priceListFields }]`,
].join(',');

export interface IDetailedOffer extends IOffer {
  tag: string;
  billingMode: BillingMode;
  currencyID: number;
  isCatalog: boolean;
  pricingMethod: string;
  unit: BillingUnit;
  productId: number;
  forecastMethod: string;
  activeContractNumber: number;
  currency: IOfferCurrency;
  product: IOfferProduct;
  priceLists: IPriceList[];
}
