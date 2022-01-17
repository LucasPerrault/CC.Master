import { BillingStrategy, CountCode, countFields, ICount } from '@cc/domain/billing/counts';
import { currencyFields, ICurrency } from '@cc/domain/billing/offers';

export const detailedCountFields = [
  countFields,
  'code',
  'number',
  'accountingNumber',
  'fixedPrice',
  'unitPrice',
  'distributorRebate',
  'additionalRebate',
  'totalBillableInEuro',
  'totalLuccaInEuro',
  'billingStrategy',
  'hasDetails',
  'canDelete',
  `contractId`,
  `currency[${currencyFields}]`,
].join(',');

export interface IDetailedCount extends ICount {
  code: CountCode;
  // eslint-disable-next-line id-blacklist
  number: number;
  accountingNumber: number;
  fixedPrice: number;
  unitPrice: number;
  distributorRebate: number;
  additionalRebate: number;
  totalBillableInEuro: number;
  totalLuccaInEuro: number;
  billingStrategy: BillingStrategy;
  hasDetails: boolean;
  canDelete: boolean;
  contractId: number;
  currency: ICurrency;
}
