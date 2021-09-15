import { clientFields, IClient } from '@cc/domain/billing/clients';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct, offerFields, productFields } from '@cc/domain/billing/offers';

export const draftFields = [
  'opportunityLineItemReccuringId',
  'opportunityLineItemDetail[productName]',
  'clientRebate',
  'theoricalStartOn',
  `product[${productFields}]`,
  `client[${clientFields}]`,
  `distributor[${distributorFields}]`,
  `offer[${offerFields}]`,
].join(',');

export interface IContractDraft {
  id: number;
  createdAt: Date;
  externalUrl: string;
  opportunityLineItemReccuringId: number;
  opportunityLineItem: IOpportunityLineItem;
  clientRebate: number;
  theoricalStartOn: string;
  product: IProduct;
  offer: IOffer;
  client: IClient;
  distributor: IDistributor;
}

interface IOpportunityLineItem {
  productName: string;
}
