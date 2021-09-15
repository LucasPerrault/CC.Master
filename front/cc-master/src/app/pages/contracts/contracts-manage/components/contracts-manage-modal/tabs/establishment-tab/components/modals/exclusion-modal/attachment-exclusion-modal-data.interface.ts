import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentContractProduct } from '../../../models/establishment-contract-product.interface';

export interface IAttachmentExclusionModalData {
  establishments: IContractEstablishment[];
  product: IEstablishmentContractProduct;
}
