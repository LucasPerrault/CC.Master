import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { IEnvironmentLog, LogsService } from '@cc/domain/environments';
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { debounceTime, map, take, takeUntil } from 'rxjs/operators';

import { ILogsFilter } from './models/logs-filter.interface';
import { LogsApiMappingService } from './services/logs-api-mapping.service';
import { LogsFilterRoutingService } from './services/logs-filter-routing.service';
import { LogsRoutingService } from './services/logs-routing.service';

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
})
export class LogsComponent implements OnInit, OnDestroy {
  public filters$: Subject<ILogsFilter> = new Subject<ILogsFilter>();
  public sortParams$: BehaviorSubject<ISortParams> = new BehaviorSubject<ISortParams>({
    field: 'createdOn',
    order: SortOrder.Asc,
  });

  private destroySubscription$: Subject<void> = new Subject<void>();
  private paginatedLogs: PaginatedList<IEnvironmentLog>;

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this.paginatedLogs.items$;
  }

  public get logsState$(): Observable<PaginatedListState> {
    return this.paginatedLogs.state$;
  }

  constructor(
    private logsService: LogsService,
    private logsApiMappingService: LogsApiMappingService,
    private logsRoutingService: LogsRoutingService,
    private logsFilterRoutingService: LogsFilterRoutingService,
    private pagingService: PagingService,
  ) {
  }

  public ngOnInit(): void {
    combineLatest([this.sortParams$, this.filters$])
      .pipe(
        takeUntil(this.destroySubscription$),
        debounceTime(300),
        map(([sort, filter]) => ({ sort, filter })),
        map(attributes => this.logsApiMappingService.toHttpParams(attributes)),
      )
      .subscribe(httpParams => this.paginatedLogs.updateHttpParams(httpParams));

    const routingParams = this.logsRoutingService.getLogsRoutingParams();
    this.logsFilterRoutingService.toLogsFilter$(routingParams)
      .pipe(take(1))
      .subscribe(f => this.filters$.next(f));

    this.paginatedLogs = this.pagingService.paginate<IEnvironmentLog>(
      (httpParams) => this.getPaginatedLogs$(httpParams),
    );
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public async updateFiltersAsync(filters: ILogsFilter): Promise<void> {
    this.filters$.next(filters);
    const routingParams = this.logsFilterRoutingService.toLogsRoutingParams(filters);
    await this.logsRoutingService.updateRouterAsync(routingParams);
  }

  public updateSort(sortParams: ISortParams) {
    this.sortParams$.next(sortParams);
  }

  public showMore(): void {
    this.paginatedLogs.nextPage();
  }

  private getPaginatedLogs$(httpParams: HttpParams): Observable<IPaginatedResult<IEnvironmentLog>> {
    return this.logsService.getLogs$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }
}
