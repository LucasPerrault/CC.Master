import { IProduct } from '@cc/domain/billing/offers';

export interface IContractProduct extends IProduct {
  id: number;
  name: string;
  code: string;
  applicationCode: string;
  parentId: number;
  isEligibleToMinimalBilling: boolean;
  isMultiSuite: boolean;
  isPromoted: boolean;
  salesForceCode: string;
  isFreeUse: boolean;
  deployRoute: string;
  familyId: number;
}
