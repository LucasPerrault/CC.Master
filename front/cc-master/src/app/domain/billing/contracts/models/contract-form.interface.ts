import { IClient } from '@cc/domain/billing/clients';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct } from '@cc/domain/billing/offers';

import { IClientRebate } from '../../../../pages/contracts/common/client-rebate/client-rebate.interface';
import { ContractBillingMonth } from '../enums/contract-billing-month.enum';

export interface IContractForm {
  billingMonth: ContractBillingMonth;
  distributor: IDistributor;
  client: IClient;
  offer: IOffer;
  product: IProduct;
  theoreticalDraftCount: number;
  clientRebate: IClientRebate;
  theoreticalMonthRebate: number;
  theoreticalStartOn: Date;
  minimalBillingPercentage: number;
  comment: string;
}
