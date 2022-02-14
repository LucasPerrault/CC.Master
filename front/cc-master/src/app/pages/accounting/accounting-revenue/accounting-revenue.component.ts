import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { BillingEntity, getBillingEntity } from '@cc/domain/billing/clients';
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
  ) { }

  public ngOnInit(): void {
    this.refreshAccountingPeriod();
    this.refreshSyncRevenueInfo();
  }

  public syncRevenue(entity: BillingEntity): void {
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

	public getSyncRevenueInfo$(billingEntity: BillingEntity): Observable<CurrentSyncRevenueInfo> {
		return this.syncRevenueInfos$.pipe(map(syncRevenuesInfos => syncRevenuesInfos.find(s => s.entity === billingEntity)));
	}

	public getBillingEntityName(billingEntity: BillingEntity): string {
		const translationKey = getBillingEntity(billingEntity)?.name;
		return this.translatePipe.transform(translationKey);
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
