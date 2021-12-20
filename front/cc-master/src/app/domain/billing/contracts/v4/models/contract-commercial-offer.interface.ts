import { IOffer } from '@cc/domain/billing/offers';

import { IContractProduct } from './contract-product.interface';

export interface IContractCommercialOffer extends IOffer {
  productId: number;
  product: IContractProduct;
}

