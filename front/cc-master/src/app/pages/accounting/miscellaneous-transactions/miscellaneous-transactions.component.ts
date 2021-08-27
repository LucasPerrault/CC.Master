import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IContract } from '@cc/domain/billing/contracts';
import { LuSidepanel } from '@lucca-front/ng/sidepanel';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, startWith, take, takeUntil } from 'rxjs/operators';

import {
  MiscTransactionCreationModalComponent,
} from './components/misc-transaction-creation-modal/misc-transaction-creation-modal.component';
import { IMiscellaneousTransaction } from './models/miscellaneous-transaction.interface';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';

@Component({
  selector: 'cc-miscellaneous-transactions',
  templateUrl: './miscellaneous-transactions.component.html',
})
export class MiscellaneousTransactionsComponent implements OnInit, OnDestroy {

  public transactions$: ReplaySubject<IMiscellaneousTransaction[]> = new ReplaySubject<IMiscellaneousTransaction[]>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public selectedTransactions: IMiscellaneousTransaction[] = [];

  public billButtonState$: Subject<string> = new Subject();

  public formControl: FormControl = new FormControl();
  public readonly contractsEndPoint = '/api/v3/newcontracts';

  public get shouldDisplayBillingAction(): boolean {
    return this.selectedTransactions.length > 0;
  }

  public get amountBillable(): number {
    return this.selectedTransactions.map(l => l.amount).reduce((a: number, b: number) => a + b, 0);
  }

  public get billingCurrencyId(): string {
    const first = this.selectedTransactions[0];
    return this.selectedTransactions.every(t => t.currencyId === first.currencyId)
      ? first.currencyId : null;
  }

  public get hasBillingClientIdEquals(): boolean {
    const first = this.selectedTransactions[0];
    return this.selectedTransactions.every(t => t.contract.clientId === first.contract.clientId);
  }

  private destroy$: Subject<void> = new Subject();

  constructor(
    private luSidepanel: LuSidepanel,
    private transactionsService: MiscellaneousTransactionsService,
  ) { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$), startWith([]))
      .subscribe(contracts => this.updateTransactions(contracts));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public openCreationModal(): void {
    const dialog = this.luSidepanel.open(MiscTransactionCreationModalComponent);
    dialog.onClose.subscribe( () => this.updateTransactions(this.formControl.value));
  }

  public updateSelectedTransactions(transactions: IMiscellaneousTransaction[]): void {
    this.selectedTransactions = transactions;
  }

  public cancelTransaction(transaction: IMiscellaneousTransaction): void {
    this.transactionsService.cancelMiscellaneousTransaction$(transaction?.id)
      .pipe(take(1))
      .subscribe(() => this.updateTransactions());
  }

  public billTransaction(): void {
    const transactionIds = this.selectedTransactions.map(t => t.id);
    this.transactionsService.billMiscellaneousTransaction$(transactionIds)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.updateTransactions(this.formControl.value)),
      )
      .subscribe(buttonState => this.billButtonState$.next(buttonState));
  }

  private updateTransactions(contracts?: IContract[]): void {
    this.resetSelectedTransactions();

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

  private resetSelectedTransactions(): void {
    this.selectedTransactions = [];
  }
}
