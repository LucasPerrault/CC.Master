import { IContract } from '@cc/domain/billing/contracts';

import { MiscTransactionInvoiceType } from '../enums/misc-transaction-invoice-type.enum';

export interface IMiscellaneousTransactionForm {
  contract: IContract;
  documentLabel: string;
  invoiceType: MiscTransactionInvoiceType;
  amount: number;
  periodOn: string;
  comment: string;
}
