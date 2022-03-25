import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { BILLING_CORE_DATA, getNameById, IBillingCoreData } from '@cc/domain/billing/billing-core-data';
import { Observable,ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { AccountingPeriodService, CurrentAccountingPeriod } from './services/accounting-period.service';
import { CurrentSyncRevenueInfo,SyncRevenueService } from './services/sync-revenue.service';

@Component({
  selector: 'cc-accounting-revenue',
  templateUrl: './accounting-revenue.component.html',
  styleUrls: ['./accounting-revenue.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AccountingRevenueComponent implements OnInit {
  public syncRevenueInfos$: ReplaySubject<CurrentSyncRevenueInfo[]> = new ReplaySubject<CurrentSyncRevenueInfo[]>(1);
  public isSyncRevenueLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public syncButtonState$: Subject<string> = new Subject<string>();

  public currentPeriod$: ReplaySubject<CurrentAccountingPeriod[]> = new ReplaySubject<CurrentAccountingPeriod[]>(1);
  public isCurrentPeriodLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public closePeriodButtonState$: Subject<string> = new Subject<string>();

  constructor(
    private accountingPeriodService: AccountingPeriodService,
		private syncRevenueService: SyncRevenueService,
		private translatePipe: TranslatePipe,
    @Inject(BILLING_CORE_DATA) private billingCoreData: IBillingCoreData,
  ) { }

  public ngOnInit(): void {
    this.refreshAccountingPeriod();
    this.refreshSyncRevenueInfo();
  }

  public syncRevenue(entityCode: string): void {
    this.syncRevenueService.synchronise$(entityCode)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refreshSyncRevenueInfo()),
      )
      .subscribe(buttonState => this.syncButtonState$.next(buttonState));
  }

  public closeCurrentPeriod(currentPeriod: Date, entityCode: string): void {
    this.accountingPeriodService.closePeriod$(currentPeriod, entityCode)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refreshAccountingPeriod()),
      )
      .subscribe(buttonState => this.closePeriodButtonState$.next(buttonState));
  }

  public getSyncRevenueInfo$(entityCode: string): Observable<CurrentSyncRevenueInfo> {
    return this.syncRevenueInfos$.pipe(map(syncRevenuesInfos => syncRevenuesInfos.find(s => s.entity === entityCode)));
	}

	public getBillingEntityName(id: number): string {
    return getNameById(this.billingCoreData.billingEntities, id);
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
      .subscribe(info => this.syncRevenueInfos$.next(info));
  }
}
