import { Component, OnInit } from '@angular/core';
import { forkJoin, Observable, ReplaySubject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { ISyncRevenueInfo } from './models/sync-revenue-info.interface';
import { AccountingPeriodService } from './services/accounting-period.service';
import { SyncRevenueService } from './services/sync-revenue.service';

@Component({
  selector: 'cc-accounting-revenue',
  templateUrl: './accounting-revenue.component.html',
  styleUrls: ['./accounting-revenue.component.scss'],
})
export class AccountingRevenueComponent implements OnInit {
  public syncRevenueInfo$: ReplaySubject<ISyncRevenueInfo> = new ReplaySubject<ISyncRevenueInfo>(1);
  public isSyncRevenueLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public currentPeriod$: ReplaySubject<Date> = new ReplaySubject<Date>(1);
  public isCurrentPeriodLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(
    private accountingPeriodService: AccountingPeriodService,
    private syncRevenueService: SyncRevenueService,
  ) { }

  public ngOnInit(): void {
    this.isCurrentPeriodLoading$.next(true);
    this.accountingPeriodService.getCurrentAccountingPeriod$()
      .pipe(take(1), finalize(() => this.isCurrentPeriodLoading$.next(false)))
      .subscribe(period => this.currentPeriod$.next(period));

    this.isSyncRevenueLoading$.next(true);
    this.syncRevenueService.getSyncInfo$()
      .pipe(take(1), finalize(() => this.isSyncRevenueLoading$.next(false)))
      .subscribe(info => this.syncRevenueInfo$.next(info));
  }
}
