import { Component, OnInit } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { finalize, take } from 'rxjs/operators';

import { IMiscellaneousTransaction } from './models/miscellaneous-transaction.interface';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';

@Component({
  selector: 'cc-miscellaneous-transactions',
  templateUrl: './miscellaneous-transactions.component.html',
})
export class MiscellaneousTransactionsComponent implements OnInit {

  public transactions$: ReplaySubject<IMiscellaneousTransaction[]> = new ReplaySubject<IMiscellaneousTransaction[]>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(private transactionsService: MiscellaneousTransactionsService) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.transactionsService.getMiscellaneousTransactions$()
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(transactions => this.transactions$.next(transactions));
  }

}
