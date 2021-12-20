import { clientFields, IClient } from '@cc/domain/billing/clients';
import { ContractBillingMonth, contractFields, IContract } from '@cc/domain/billing/contracts';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct, productFields } from '@cc/domain/billing/offers';

export const contractDetailedFields = [
  contractFields,
  'billingMonth',
  `distributor[${distributorFields}]`,
  `client[${clientFields},salesforceId,commercialManagementId]`,
  `offer[id,name]`,
  `product[${productFields}]`,
  'unityNumberTheorical',
  'clientRebate',
  'endClientRebateOn',
  'nbMonthTheorical',
  'theoricalStartOn',
  'minimalBillingPercentage',
  'comment',
].join(',');

export interface IClientWithExternalInformation extends IClient {
  salesforceId: string;
  commercialManagementId: string;
}

export interface IContractDetailed extends IContract {
  billingMonth: ContractBillingMonth;
  distributor: IDistributor;
  client: IClientWithExternalInformation;
  offer: IOffer;
  product: IProduct;
  unityNumberTheorical: number;
  clientRebate: number;
  endClientRebateOn: Date;
  nbMonthTheorical: number;
  theoricalStartOn: Date;
  minimalBillingPercentage: number;
  comment: string;
}
