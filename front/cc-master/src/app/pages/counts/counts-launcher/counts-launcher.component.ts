import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { PagingService } from '@cc/common/paging';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { LuModal } from '@lucca-front/ng/modal';
import { startOfMonth, subMonths } from 'date-fns';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, finalize, map, take, takeUntil } from 'rxjs/operators';

import { CountsProcessLauncherModalComponent } from './components/counts-process-launcher-modal/counts-process-launcher-modal.component';
import { ICountsDashboard, toDashboard } from './models/counts-dashboard.interface';
import { CountsDataService } from './services/counts-data.service';
import { CountsProcessDataService } from './services/counts-process-data.service';

@Component({
  selector: 'cc-counts-launcher',
  templateUrl: './counts-launcher.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsLauncherComponent implements OnInit, OnDestroy {

  public dashboard$ = new ReplaySubject<ICountsDashboard>();
  public isLoading$ = new ReplaySubject<boolean>();

  public countPeriod: FormControl = new FormControl();
  public granularity = ELuDateGranularity;

  public hasRunningCounts$ = new ReplaySubject<boolean>(1);

  public cleanForecastButtonClass$ = new ReplaySubject<string>(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private processService: CountsProcessDataService,
    private dataService: CountsDataService,
    private pagingService: PagingService,
    private luModal: LuModal,
  ) {}

  public ngOnInit(): void {
    this.refreshRunningProcess();

    this.countPeriod.valueChanges
      .pipe(takeUntil(this.destroy$), filter(period => !!period))
      .subscribe(countPeriod => this.resetDashboard(countPeriod));

    const defaultCountPeriod = subMonths(startOfMonth(Date.now()), 1);
    this.countPeriod.setValue(defaultCountPeriod);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public cleanForecast(): void {
    this.dataService.cleanForecast$()
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(this.cleanForecastButtonClass$);
  }

  public openCountsLauncher(): void {
    const modalRef = this.luModal.open(CountsProcessLauncherModalComponent);
    modalRef.onClose.pipe(takeUntil(this.destroy$)).subscribe(() => this.refreshRunningProcess());
  }

  private refreshRunningProcess(): void {
    this.processService.getCountProcessNumber$()
      .pipe(take(1), map(processNumber => !!processNumber))
      .subscribe(this.hasRunningCounts$);
  }

  private resetDashboard(countPeriod: Date): void {
    this.isLoading$.next(true);

    combineLatest([
      this.dataService.getActiveContractsNumber$(countPeriod),
      this.dataService.getRealCountsNumber$(countPeriod),
      this.getContractIdsWithoutCount$(countPeriod),
      this.getContractIdsWithDraftCount$(countPeriod),
      this.getContractIdsWithCountWithoutAccountingEntry$(countPeriod),
    ])
      .pipe(take(1), toDashboard(countPeriod), finalize(() => this.isLoading$.next(false)))
      .subscribe(dashboard => this.dashboard$.next(dashboard));
  }

  private getContractIdsWithDraftCount$(countPeriod: Date): Observable<number[]> {
    return this.dataService.getDraftCounts$(countPeriod).pipe(map(count => count.map(c => c.contractID)));
  }

  private getContractIdsWithCountWithoutAccountingEntry$(countPeriod: Date): Observable<number[]> {
    return this.dataService.getRealCountsWithoutAccountingEntries(countPeriod).pipe(map(count => count.map(c => c.contractID)));
  }

  private getContractIdsWithoutCount$(countPeriod: Date): Observable<number[]> {
    return this.dataService.getMissingCounts$(countPeriod).pipe(map(count => count.map(c => c.contractId)));
  }
}
