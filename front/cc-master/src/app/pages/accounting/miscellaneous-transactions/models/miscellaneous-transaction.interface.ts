import { IMiscellaneousTransactionAuthor, miscTransactionAuthorFields } from './miscellaneous-transaction-author.interface';
import { IMiscellaneousTransactionContract, miscTransactionContractFields } from './miscellaneous-transaction-contract.interface';
import {
  IMiscellaneousTransactionContractEntry,
  miscTransactionContractEntryFields,
} from './miscellaneous-transaction-contract-entry.interface';

export const miscTransactionFields = [
  'id',
  'currencyId',
  'amount',
  'comment',
  'createdAt',
  'isCanceled',
  'createdAt',
  'periodOn',
  'documentLabel',
  'hasEntryLettered',
  'isCancelable',
  `author[${ miscTransactionAuthorFields }]`,
  `contract[${ miscTransactionContractFields }]`,
  `entries[${ miscTransactionContractEntryFields }]`,
].join(',');

export interface IMiscellaneousTransaction {
  id: number;
  currencyId: string;
  amount: number;
  comment: string;
  author: IMiscellaneousTransactionAuthor;
  isCanceled: boolean;
  contract: IMiscellaneousTransactionContract;
  periodOn: Date;
  createdAt: Date;
  documentLabel: string;
  hasEntryLettered: boolean;
  isCancelable: boolean;
  entries: IMiscellaneousTransactionContractEntry[];
}
