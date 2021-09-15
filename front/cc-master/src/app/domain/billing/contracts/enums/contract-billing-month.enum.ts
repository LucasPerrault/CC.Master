import { IContractBillingMonth } from '@cc/domain/billing/contracts';

export enum ContractBillingMonth {
  January = 1,
  February = 2,
  March = 3,
  April = 4,
  May	= 5,
  June = 6,
  July = 7,
  August = 8,
  September = 9,
  October = 10,
  November = 11,
  December = 12,
  Quarterly = 13,
}

export const contractBillingMonths: IContractBillingMonth[] = [
  {
    id: ContractBillingMonth.January,
    name: 'front_contract_billingMonth_january',
  },
  {
    id: ContractBillingMonth.February,
    name: 'front_contract_billingMonth_february',
  },
  {
    id: ContractBillingMonth.March,
    name: 'front_contract_billingMonth_march',
  },
  {
    id: ContractBillingMonth.April,
    name: 'front_contract_billingMonth_april',
  },
  {
    id: ContractBillingMonth.May,
    name: 'front_contract_billingMonth_may',
  },
  {
    id: ContractBillingMonth.June,
    name: 'front_contract_billingMonth_june',
  },
  {
    id: ContractBillingMonth.July,
    name: 'front_contract_billingMonth_july',
  },
  {
    id: ContractBillingMonth.August,
    name: 'front_contract_billingMonth_august',
  },
  {
    id: ContractBillingMonth.September,
    name: 'front_contract_billingMonth_september',
  },
  {
    id: ContractBillingMonth.October,
    name: 'front_contract_billingMonth_october',
  },
  {
    id: ContractBillingMonth.November,
    name: 'front_contract_billingMonth_november',
  },
  {
    id: ContractBillingMonth.December,
    name: 'front_contract_billingMonth_december',
  },
];
