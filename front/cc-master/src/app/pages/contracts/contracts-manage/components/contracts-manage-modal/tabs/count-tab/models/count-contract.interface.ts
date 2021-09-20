import { contractFields, IContract } from '@cc/domain/billing/contracts';

export const countContractFields = `${ contractFields },minimalBillingPercentage,isDirectSales,theoricalStartOn,closeOn`;

export interface ICountContract extends IContract {
  theoricalStartOn: string;
  minimalBillingPercentage: number;
  isDirectSales: boolean;
  closeOn: string;
}
