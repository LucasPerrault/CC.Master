import { contractFields, IContract } from '@cc/domain/billing/contracts';

export const miscTransactionContractFields = `${ contractFields },clientId`;

export interface IMiscellaneousTransactionContract extends IContract {
  clientId: number;
}
