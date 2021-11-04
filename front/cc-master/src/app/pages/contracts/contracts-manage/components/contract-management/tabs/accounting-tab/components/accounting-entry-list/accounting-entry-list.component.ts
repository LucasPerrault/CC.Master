import { Component, Input } from '@angular/core';
import { PaginatedListState } from '@cc/common/paging';

import { AccountingEntryJournalCodes } from '../../enums/accounting-entry-journal-code.enum';
import { IAccountingEntry } from '../../models/accounting-entry.interface';

@Component({
  selector: 'cc-accounting-entry-list',
  templateUrl: './accounting-entry-list.component.html',
  styleUrls: ['./accounting-entry-list.component.scss'],
})
export class AccountingEntryListComponent {
  @Input() accountingEntries: IAccountingEntry[];
  @Input() state: PaginatedListState;

  public isBill(journalCode: AccountingEntryJournalCodes): boolean {
    return journalCode === AccountingEntryJournalCodes.Bill;
  }

  public get isUpdated(): boolean {
    return this.state === PaginatedListState.Update;
  }

  public get isLoadingMore(): boolean {
    return this.state === PaginatedListState.LoadMore;
  }

  public get isEmpty(): boolean {
    return this.isIdle && !this.accountingEntries.length;
  }

  public get isIdle(): boolean {
    return this.state === PaginatedListState.Idle || this.state === PaginatedListState.Error;
  }

  public isLettered(entry: IAccountingEntry): boolean {
    return entry.letter !== null;
  }

  public trackBy(index: number, accountingEntry: IAccountingEntry): number {
    return accountingEntry.id;
  }
}
