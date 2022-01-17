import { clientFields, IClient } from '@cc/domain/billing/clients';
import { contractFields, IContract } from '@cc/domain/billing/contracts';
import { BillingStrategy, countFields, ICount } from '@cc/domain/billing/counts';
import { distributorFields, IDistributor } from '@cc/domain/billing/distributors';
import { currencyFields, ICurrency, IOffer } from '@cc/domain/billing/offers';

const countContractFields = [
  contractFields,
  'minimalBillingPercentage',
  `client[${ clientFields }]`,
  `distributor[${ distributorFields }]`,
].join(',');
export interface ICountContract extends IContract {
  client: IClient;
  distributor: IDistributor;
  minimalBillingPercentage: number;
}

export const detailedCountFields = [
  countFields,
  'collection.count',
  'number',
  'accountingNumber',
  'billingStrategy',
  'unitPrice',
  'totalLucca',
  'totalPartner',
  'totalBillableInEuro',
  'totalLuccaInEuro',
  'totalPartnerInEuro',
  'totalBillable',
  'additionalRebate',
  'distributorRebate',

  `contract[${ countContractFields }]`,
  `offer[id,name]`,
  `currency[${ currencyFields }]`,
].join(',');

export interface IDetailedCount extends ICount {
  // eslint-disable-next-line id-blacklist
  number: number;
  accountingNumber: number;
  billingStrategy: BillingStrategy;
  unitPrice: number;
  totalLucca: number;
  totalPartner: number;
  totalBillableInEuro: number;
  totalLuccaInEuro: number;
  totalPartnerInEuro: number;
  totalBillable: number;
  additionalRebate: number;
  distributorRebate: number;

  contract: ICountContract;
  offer: IOffer;
  currency: ICurrency;
}
