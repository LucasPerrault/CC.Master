import { clientFields, IClient } from '@cc/domain/billing/clients';
import { contractFields, IContract } from '@cc/domain/billing/contracts/models/contract.interface';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { establishmentFields, IEstablishment } from '@cc/domain/billing/establishments';
import { IOffer, IProduct, offerFields, productFields } from '@cc/domain/billing/offers';

const productCodeFields = `${ productFields },code`;
interface IProductCode extends IProduct {
  code: string;
}

export const contractListEntryFields = [
  contractFields,
  'collection.count',
  'createdOn',
  'startOn',
  'theoricalStartOn',
  'closeOn',
  'leErrorNumber',
  'environmentId',
  `activeLegalEntities[${ establishmentFields }]`,
  `product[${ productCodeFields }]`,
  `offer[${ offerFields }]`,
  `client[${ clientFields }]`,
  `distributor[${ distributorFields }]`,
].join(',');

export interface IContractListEntry extends IContract {
  createdOn: string;
  startOn: string;
  theoricalStartOn: string;
  closeOn: string;
  leErrorNumber: number;
  environmentId: number;
  activeLegalEntities: IEstablishment[];
  product: IProductCode;
  offer: IOffer;
  client: IClient;
  distributor: IDistributor;
}
