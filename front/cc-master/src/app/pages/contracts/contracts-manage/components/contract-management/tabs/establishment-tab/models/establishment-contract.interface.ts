import { contractFields, IContract } from '@cc/domain/billing/contracts';

import { etsContractProductFields, IEstablishmentContractProduct } from './establishment-contract-product.interface';

export const establishmentContractFields = [
  contractFields,
  'environmentId',
  'theoricalStartOn',
  'nbMonthTheorical',
  'closeOn',
  'productId',
  `product[${ etsContractProductFields }]`,
].join(',');

export interface IEstablishmentContract extends IContract {
  environmentId: number;
  productId: number;
  product: IEstablishmentContractProduct;
  theoricalStartOn: string;
  closeOn: string;
  nbMonthTheorical: number;
}
