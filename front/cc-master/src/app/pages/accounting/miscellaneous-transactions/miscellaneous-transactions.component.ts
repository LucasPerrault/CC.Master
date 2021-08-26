import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { IContract } from '@cc/domain/billing/contracts';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, startWith, switchMap, take, takeUntil } from 'rxjs/operators';

import { IMiscellaneousTransaction } from './models/miscellaneous-transaction.interface';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';

@Component({
  selector: 'cc-miscellaneous-transactions',
  templateUrl: './miscellaneous-transactions.component.html',
})
export class MiscellaneousTransactionsComponent implements OnInit, OnDestroy {

  public transactions$: ReplaySubject<IMiscellaneousTransaction[]> = new ReplaySubject<IMiscellaneousTransaction[]>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public formControl: FormControl = new FormControl();
  public readonly contractsEndPoint = '/api/v3/newcontracts';

  private destroy$: Subject<void> = new Subject();

  constructor(private transactionsService: MiscellaneousTransactionsService) { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$), startWith([]))
      .subscribe(contracts => this.updateTransactions(contracts));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public cancelTransaction(transaction: IMiscellaneousTransaction): void {
    this.transactionsService.cancelMiscellaneousTransaction$(transaction?.id)
      .pipe(take(1))
      .subscribe(() => this.updateTransactions());
  }

  private updateTransactions(contracts?: IContract[]): void {
    this.getMiscellaneousTransactions$(contracts)
      .pipe(take(1))
      .subscribe(transactions => this.transactions$.next(transactions));
  }

  private getMiscellaneousTransactions$(contracts: IContract[]): Observable<IMiscellaneousTransaction[]> {
    this.isLoading$.next(true);

    const contractIds = contracts?.map(c => c.id);
    return this.transactionsService.getMiscellaneousTransactions$(contractIds)
      .pipe(finalize(() => this.isLoading$.next(false)));
  }
}
