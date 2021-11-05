import { contractFields, IContract } from '@cc/domain/billing/contracts';

import { CloseContractReason } from '../constants/close-contract-reason.enum';

export const contractClosureDetailedFields = [
  contractFields,
  'closeOn',
  'closeReason',
  'theoricalStartOn',
].join(',');

export interface IContractClosureDetailed extends IContract {
  closeOn: string;
  closeReason: CloseContractReason;
  theoricalStartOn: string;
}
