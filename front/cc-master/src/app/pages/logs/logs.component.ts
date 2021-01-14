import { Component, OnDestroy, OnInit } from '@angular/core';
import { IApiV3SortParams, IHttpQueryParams } from '@cc/common/queries';
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IEnvironmentLog } from './models';
import { LogsService } from './services';

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
})
export class LogsComponent implements OnInit, OnDestroy {
  public defaultSortParams: IApiV3SortParams = {
    field: 'createdOn',
    order: 'desc',
  };
  private queryFilterParams$: BehaviorSubject<IHttpQueryParams> = new BehaviorSubject<IHttpQueryParams>(null);
  private sortParams$: BehaviorSubject<IApiV3SortParams> = new BehaviorSubject<IApiV3SortParams>(this.defaultSortParams);
  private destroySubscription$: Subject<void> = new Subject<void>();

  constructor(private logsService: LogsService) {}

  public ngOnInit(): void {
    this.refreshLogsWhenQueryParamsChange();
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public updateQueryFilters(queryFiltersParams: IHttpQueryParams): void {
    this.queryFilterParams$.next(queryFiltersParams);
  }

  public sortBy(sortParams: IApiV3SortParams) {
    this.sortParams$.next(sortParams);
  }

  public async showMoreDataAsync(): Promise<void> {
    await this.logsService.showMoreDataAsync(this.sortParams$.value, this.queryFilterParams$.value);
  }

  private refreshLogsWhenQueryParamsChange(): void {
    combineLatest([this.queryFilterParams$, this.sortParams$])
      .pipe(takeUntil(this.destroySubscription$))
      .subscribe(async ([queryFilterParams, sortParams]) =>
        await this.logsService.refreshLogsAsync(sortParams, queryFilterParams),
      );
  }

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this.logsService.logs$;
  }

  public get isShownMoreDataLoading$(): Observable<boolean> {
    return this.logsService.isShownMoreDataLoading$;
  }

  public get isRefreshedDataLoading$(): Observable<boolean> {
    return this.logsService.isRefreshedDataLoading$;
  }
}
