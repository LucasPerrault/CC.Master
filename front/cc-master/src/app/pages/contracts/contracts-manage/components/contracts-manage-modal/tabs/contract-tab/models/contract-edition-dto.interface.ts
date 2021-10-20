import { ContractBillingMonth } from '@cc/domain/billing/contracts';

export interface IContractEditionDto {
  billingMonth: ContractBillingMonth;
  distributorId: string;
  clientId: number;
  offerId: number;
  unityNumberTheorical: number;
  clientRebate: number;
  endClientRebateOn: Date;
  nbMonthTheorical: number;
  theoricalStartOn: Date;
  minimalBillingPercentage: number;
  comment: string;
}
