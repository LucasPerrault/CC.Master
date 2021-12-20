import { clientFields, IClient } from '@cc/domain/billing/clients';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct, productFields } from '@cc/domain/billing/offers';

import { draftFields, IContractDraft } from './contract-draft.interface';

export const draftListEntryFields = [
  draftFields,
  'clientRebate',
  'theoricalStartOn',
  `product[${productFields}]`,
  `client[${clientFields}]`,
  `distributor[${distributorFields}]`,
  `offer[id,name]`,
].join(',');

export interface IContractDraftListEntry extends IContractDraft {
  createdAt: Date;
  externalUrl: string;
  clientRebate: number;
  theoricalStartOn: string;
  product: IProduct;
  offer: IOffer;
  client: IClient;
  distributor: IDistributor;
}
