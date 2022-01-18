import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { IContract } from '@cc/domain/billing/contracts';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { startOfMonth, subMonths } from 'date-fns';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, finalize, map, take, takeUntil, tap } from 'rxjs/operators';

import { ContractState } from '../../contracts/contracts-manage/constants/contract-state.enum';
import { CountsDashboardDetailsSection } from './enums/counts-dashboard-details-section.enum';
import { CountsDashboard } from './models/counts-dashboard.interface';
import { CountsDataService } from './services/counts-data.service';
import { CountsLauncherService } from './services/counts-launcher.service';

@Component({
  selector: 'cc-counts-launcher',
  templateUrl: './counts-launcher.component.html',
  styleUrls: ['./counts-launcher.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsLauncherComponent implements OnInit, OnDestroy {

  public dashboard: CountsDashboard = new CountsDashboard();

  public countPeriod: FormControl = new FormControl();
  public granularity = ELuDateGranularity;

  public selectedSection$ = new ReplaySubject<CountsDashboardDetailsSection>(1);
  public section = CountsDashboardDetailsSection;

  private paginatedContractsWithDraftCount: PaginatedList<IContract>;
  private paginatedContractsWithCountWithoutAccountingEntry: PaginatedList<IContract>;

  private destroy$: Subject<void> = new Subject();

  constructor(
    private launcherService: CountsLauncherService,
    private dataService: CountsDataService,
    private pagingService: PagingService,
  ) {}

  public ngOnInit(): void {
    this.paginatedContractsWithDraftCount = this.pagingService.paginate<IContract>(
      (httpParams) => this.launcherService.getContractsWithDraftCount$(httpParams, new Date(this.countPeriod.value)));

    this.paginatedContractsWithDraftCount.items$
      .pipe(takeUntil(this.destroy$))
      .subscribe(this.dashboard.contractsWithDraftCount$);

    this.paginatedContractsWithDraftCount.totalCount$
      .pipe(takeUntil(this.destroy$))
      .subscribe(this.dashboard.numberOfContractsWithDraftCount$);

    this.paginatedContractsWithDraftCount.state$
      .pipe(takeUntil(this.destroy$), map(state => state === PaginatedListState.Update))
      .subscribe(this.dashboard.isContractsWithDraftCountLoading$);

    this.paginatedContractsWithCountWithoutAccountingEntry = this.pagingService.paginate<IContract>(
      (httpParams) => this.launcherService.getContractsWithCountWithoutAccountingEntry$(httpParams, new Date(this.countPeriod.value)));

    this.paginatedContractsWithCountWithoutAccountingEntry.items$
      .pipe(takeUntil(this.destroy$))
      .subscribe(this.dashboard.contractsWithCountWithoutAccountingEntry$);

    this.paginatedContractsWithCountWithoutAccountingEntry.totalCount$
      .pipe(takeUntil(this.destroy$))
      .subscribe(this.dashboard.numberOfContractsWithCountWithoutAccountingEntry$);

    this.paginatedContractsWithCountWithoutAccountingEntry.state$
      .pipe(takeUntil(this.destroy$), map(state => state === PaginatedListState.Update))
      .subscribe(this.dashboard.isContractsWithCountWithoutAccountingEntryLoading$);

    this.countPeriod.valueChanges
      .pipe(takeUntil(this.destroy$), filter(period => !!period))
      .subscribe(countPeriod => this.refreshDashboard(countPeriod));

    const defaultCountPeriod = subMonths(startOfMonth(Date.now()), 1);
    this.countPeriod.setValue(defaultCountPeriod);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public showSectionDetails(section: CountsDashboardDetailsSection): void {
    this.selectedSection$.next(section);
  }

  public nextPage(section: CountsDashboardDetailsSection): void {
    switch (section) {
      case CountsDashboardDetailsSection.WithDraftCount:
        this.paginatedContractsWithDraftCount.nextPage();
        return;
      case CountsDashboardDetailsSection.WithCountWithoutAccountingEntry:
        this.paginatedContractsWithCountWithoutAccountingEntry.nextPage();
        return;
    }
  }

  public async redirectToContractsAsync(section: CountsDashboardDetailsSection): Promise<void> {
    const countPeriod = new Date(this.countPeriod.value);

    switch (section) {
      case CountsDashboardDetailsSection.WithoutCount:
        this.dashboard.contractsWithoutCounts$
          .pipe(take(1), map(contracts => contracts.map(c => c?.id)))
          .subscribe(ids => this.redirectToContracts(ids));
        return;
      case CountsDashboardDetailsSection.WithDraftCount:
        this.getAllContractIdsWithDraftCount$(countPeriod)
          .pipe(tap(ids => this.redirectToContracts(ids)), toSubmissionState())
          .subscribe(this.dashboard.withDraftCountRedirectionState$);
        return;
      case CountsDashboardDetailsSection.WithCountWithoutAccountingEntry:
        this.getAllContractIdsWithCountWithoutAccountingEntry$(countPeriod)
          .pipe(tap(ids => this.redirectToContracts(ids)), toSubmissionState())
          .subscribe(this.dashboard.withCountWithoutAccountingEntryRedirectionState$);
        return;
    }
  }

  private refreshDashboard(countPeriod: Date): void {
    this.dashboard.countPeriod = countPeriod;
    this.dashboard.isNumberOfContractsLoading$.next(true);
    this.dashboard.isContractsWithoutCountLoading$.next(true);
    this.dashboard.isNumberOfRealCountsLoading$.next(true);

    this.dataService.getActiveContractsNumber$(countPeriod)
      .pipe(take(1), finalize(() => this.dashboard.isNumberOfContractsLoading$.next(false)))
      .subscribe(this.dashboard.numberOfContracts$);

    this.dataService.getRealCountsNumber$(countPeriod)
      .pipe(take(1), finalize(() => this.dashboard.isNumberOfRealCountsLoading$.next(false)))
      .subscribe(this.dashboard.numberOfRealCounts$);

    this.launcherService.getContractsWithoutCount$(countPeriod)
      .pipe(take(1), finalize(() => this.dashboard.isContractsWithoutCountLoading$.next(false)))
      .subscribe(contracts => {
        this.dashboard.contractsWithoutCounts$.next(contracts);
        this.dashboard.numberOfContractsWithoutCount$.next(contracts?.length);
      });

    this.paginatedContractsWithDraftCount.updateHttpParams(new HttpParams());
    this.paginatedContractsWithCountWithoutAccountingEntry.updateHttpParams(new HttpParams());
  }

  private getAllContractIdsWithDraftCount$(countPeriod: Date): Observable<number[]> {
    return this.launcherService.getContractsWithDraftCount$(new HttpParams(), countPeriod)
      .pipe(take(1), map(res => res.items?.map(c => c?.id)));
  }

  private getAllContractIdsWithCountWithoutAccountingEntry$(countPeriod: Date): Observable<number[]> {
    return this.launcherService.getContractsWithCountWithoutAccountingEntry$(new HttpParams(), countPeriod)
      .pipe(take(1), map(res => res.items?.map(c => c?.id)));
  }

  private redirectToContracts(ids: number[]): void {
    const url = `${NavigationPath.Contracts}/${NavigationPath.ContractsManage}`;
    const query = [
      `contractIds=${ids.join(',')}`,
      `stateids=${ ContractState.NotStarted },${ ContractState.InProgress},${ ContractState.Closed }`,
    ];

    const redirectionUrl = !!ids?.length ? `${url}?${ query.join('&') }` : url;
    window.open(redirectionUrl);
  }
}
