import { clientFields, IClient } from '@cc/domain/billing/clients';
import { ContractBillingMonth } from '@cc/domain/billing/contracts';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct, productFields } from '@cc/domain/billing/offers';

import { draftFields, IContractDraft } from './contract-draft.interface';

const opportunityLineItemFields = `productName,deploymentOn`;
interface IOpportunityLineItem {
  productName: string;
  deploymentOn: Date;
}

export const draftFormFields = [
  draftFields,
  'billingMonth',
  'unityNumberTheorical',
  'clientRebate',
  'endClientRebateOn',
  'nbMonthTheorical',
  'theoricalStartOn',
  'minimalBillingPercentage',
  'comment',
  `product[${productFields}]`,
  `client[${clientFields}]`,
  `distributor[${distributorFields}]`,
  `offer[id,name]`,
  `opportunityLineItemDetail[${opportunityLineItemFields}]`,
].join(',');

export interface IContractDraftForm extends IContractDraft {
  externalDistributorUrl: string;
  billingMonth: ContractBillingMonth;
  unityNumberTheorical: number;
  clientRebate: number;
  endClientRebateOn: string;
  nbMonthTheorical: number;
  theoricalStartOn: string;
  minimalBillingPercentage: number;
  comment: string;
  distributor: IDistributor;
  client: IClient;
  offer: IOffer;
  product: IProduct;
  opportunityLineItemDetail: IOpportunityLineItem;
}
