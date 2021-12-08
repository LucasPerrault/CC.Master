import { IProduct, ISolution, productFields } from '@cc/domain/billing/offers';

export const offerProductFields = `${ productFields },parentId,isEligibleToMinimalBilling,isMultiSuite,solutions`;

export interface IOfferProduct extends IProduct {
  parentId: number;
  isEligibleToMinimalBilling: boolean;
  isMultiSuite: boolean;
  solutions: ISolution[];
}
