import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { ISyncRevenueInfo } from './models/sync-revenue-info.interface';
import { AccountingPeriodService } from './services/accounting-period.service';
import { SyncRevenueService } from './services/sync-revenue.service';
import { getButtonState, toSubmissionState } from '@cc/common/forms';

@Component({
  selector: 'cc-accounting-revenue',
  templateUrl: './accounting-revenue.component.html',
  styleUrls: ['./accounting-revenue.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AccountingRevenueComponent implements OnInit {
  public syncRevenueInfo$: ReplaySubject<ISyncRevenueInfo> = new ReplaySubject<ISyncRevenueInfo>(1);
  public isSyncRevenueLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public syncButtonState$: Subject<string> = new Subject<string>();

  public currentPeriod$: ReplaySubject<Date> = new ReplaySubject<Date>(1);
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

  public syncRevenue(): void {
    this.syncRevenueService.synchronise$()
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refreshSyncRevenueInfo()),
      )
      .subscribe(buttonState => this.syncButtonState$.next(buttonState));
  }

  public closeCurrentPeriod(currentPeriod: Date): void {
    this.accountingPeriodService.closePeriod$(currentPeriod)
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
    this.accountingPeriodService.getCurrentAccountingPeriod$()
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
