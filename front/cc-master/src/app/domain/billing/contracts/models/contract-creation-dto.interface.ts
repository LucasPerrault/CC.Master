import { ContractBillingMonth } from '../enums/contract-billing-month.enum';

export interface IContractCreationDto {
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
