import { IProduct, ISolution, productFields } from '@cc/domain/billing/offers';

export const etsContractProductFields = `${ productFields },isMultiSuite,solutions`;

export interface IEstablishmentContractProduct extends IProduct {
  isMultiSuite: boolean;
  solutions: ISolution[];
}
