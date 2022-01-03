import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { ISyncRevenueInfo } from './models/sync-revenue-info.interface';
import { AccountingPeriodService, CurrentAccountingPeriod } from './services/accounting-period.service';
import { SyncRevenueService, CurrentSyncRevenueInfo } from './services/sync-revenue.service';
import { BillingEntity } from '@cc/domain/billing/billing-entity';

@Component({
  selector: 'cc-accounting-revenue',
  templateUrl: './accounting-revenue.component.html',
  styleUrls: ['./accounting-revenue.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AccountingRevenueComponent implements OnInit {
  public syncRevenueInfo$: ReplaySubject<CurrentSyncRevenueInfo[]> = new ReplaySubject<CurrentSyncRevenueInfo[]>(1);
  public isSyncRevenueLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public syncButtonState$: Subject<string> = new Subject<string>();

  public currentPeriod$: ReplaySubject<CurrentAccountingPeriod[]> = new ReplaySubject<CurrentAccountingPeriod[]>(1);
  public isCurrentPeriodLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public closePeriodButtonState$: Subject<string> = new Subject<string>();

  constructor(
    private accountingPeriodService: AccountingPeriodService,
    private syncRevenueService: SyncRevenueService,
  ) { }

  public ngOnInit(): void {
    this.refreshAccountingPeriod();
    this.refreshSyncRevenueInfo();
  }

  public syncRevenue(entity: BillingEntity): void {
		console.log(entity);
    this.syncRevenueService.synchronise$(entity)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refreshSyncRevenueInfo()),
      )
      .subscribe(buttonState => this.syncButtonState$.next(buttonState));
  }

  public closeCurrentPeriod(currentPeriod: Date, entity: BillingEntity): void {
    this.accountingPeriodService.closePeriod$(currentPeriod, entity)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refreshAccountingPeriod()),
      )
      .subscribe(buttonState => this.closePeriodButtonState$.next(buttonState));
  }

  private refreshAccountingPeriod(): void {
    this.isCurrentPeriodLoading$.next(true);
    this.accountingPeriodService.getCurrentAccountingPeriods$()
      .pipe(take(1), finalize(() => this.isCurrentPeriodLoading$.next(false)))
      .subscribe(period => this.currentPeriod$.next(period));
  }

  private refreshSyncRevenueInfo(): void {
    this.isSyncRevenueLoading$.next(true);
    this.syncRevenueService.getSyncInfo$()
      .pipe(take(1), finalize(() => this.isSyncRevenueLoading$.next(false)))
      .subscribe(info => this.syncRevenueInfo$.next(info));
  }
}
