import { AccountingEntryJournalCodes } from '../enums/accounting-entry-journal-code.enum';

export const accountingEntryFields = [
  'id',
  'entryNumber',
  'accountNumber',
  'journalCode',
  'label',
  'reference',
  'currencyAmount',
  'euroAmount',
  'accountingDate',
  'isCredit',
  'letter',
  'periodOn',
  'collection.count',
].join(',');

export interface IAccountingEntry {
  id: number;
  entryNumber: number;
  accountNumber: string;
  journalCode: AccountingEntryJournalCodes;
  label: string;
  reference: string;
  currencyAmount: number;
  euroAmount: number;
  accountingDate: Date;
  periodOn: Date;
  isCredit: boolean;
  letter: number;
}
