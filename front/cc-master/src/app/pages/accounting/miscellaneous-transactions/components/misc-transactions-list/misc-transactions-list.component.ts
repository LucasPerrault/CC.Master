import { Component, EventEmitter, Input, Output } from '@angular/core';

import { IMiscellaneousTransaction } from '../../models/miscellaneous-transaction.interface';

@Component({
  selector: 'cc-misc-transactions-list',
  templateUrl: './misc-transactions-list.component.html',
  styleUrls: ['./misc-transactions-list.component.scss'],
})
export class MiscTransactionsListComponent {
  @Input() public transactions: IMiscellaneousTransaction[];
  @Output() public cancelTransaction: EventEmitter<IMiscellaneousTransaction> = new EventEmitter();

  public selectedTransactions: IMiscellaneousTransaction[] = [];
  public cancellingTransactions: IMiscellaneousTransaction[] = [];

  constructor() { }

  public cancel(transaction: IMiscellaneousTransaction): void {
    this.cancellingTransactions = [...this.cancellingTransactions, transaction];
    this.cancelTransaction.emit(transaction);
  }

  public isCancelling(transaction: IMiscellaneousTransaction): boolean {
    return !!this.cancellingTransactions.find(t => t.id === transaction.id);
  }

  public isSelected(transaction: IMiscellaneousTransaction): boolean {
    return !!this.selectedTransactions.find(t => t.id === transaction.id);
  }

  public select(transaction: IMiscellaneousTransaction): void {
    this.selectedTransactions = this.isSelected(transaction)
      ? this.selectedTransactions.filter(t => t.id !== transaction.id)
      : [...this.selectedTransactions, transaction];
  }

  public getAccountingDate(transaction: IMiscellaneousTransaction): string {
    const [firstEntry] = transaction.entries;
    return firstEntry.accountingDate;
  }

  public redirectToContract(transaction: IMiscellaneousTransaction): void {
    window.open(this.contractUrl(transaction.contract.id));
  }

  private contractUrl = (contractId: number) => `/contracts/manage/${ contractId }/contract`;
}
