import { IBillingEntity } from '@cc/domain/billing/clients';
import { IContract } from '@cc/domain/billing/contracts/v4';

export enum MiscTransactionsFilterKey {
  Contracts = 'contracts',
  BillingEntities = 'billingEntities',
}

export interface IMiscTransactionsFilter {
  contracts: IContract[];
  billingEntities: IBillingEntity[];
}
