import { ContractBillingMonth } from '@cc/domain/billing/contracts';

export interface IContractEditionDto {
  billingMonth: ContractBillingMonth;
  distributorId: number;
  clientId: number;
  offerId: number;
  unityNumberTheorical: number;
  clientRebate: number;
  endClientRebateOn: string;
  nbMonthTheorical: number;
  theoricalStartOn: string;
  minimalBillingPercentage: number;
  comment: string;
}
