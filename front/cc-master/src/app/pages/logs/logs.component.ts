import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { IEnvironmentLog, LogsService } from '@cc/domain/environments';
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { debounceTime, map, take, takeUntil } from 'rxjs/operators';

import { ILogsFilter } from './models/logs-filter.interface';
import { ILogsRoutingParams } from './models/logs-routing-params.interface';
import { LogsApiMappingService } from './services/logs-api-mapping.service';
import { LogsFilterRoutingService } from './services/logs-filter-routing.service';
import { LogsRoutingService } from './services/logs-routing.service';

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
})
export class LogsComponent implements OnInit, OnDestroy {
  public filters$: BehaviorSubject<ILogsFilter>;
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
    this.initLogsFilterAsync(this.logsRoutingService.getLogsRoutingParams())
      .then(() => this.initHttpParamsUpdateBySubscription());
  }

  public ngOnInit(): void {
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

  private async initLogsFilterAsync(routingParams: ILogsRoutingParams): Promise<void> {
    const logsFilter = await this.logsFilterRoutingService.toLogsFilter$(routingParams)
      .pipe(take(1))
      .toPromise();

    this.filters$ = new BehaviorSubject<ILogsFilter>(logsFilter);
  }

  private initHttpParamsUpdateBySubscription(): void {
    combineLatest([this.sortParams$, this.filters$])
      .pipe(
        takeUntil(this.destroySubscription$),
        debounceTime(300),
        map(([sortParams, filters]) => this.logsApiMappingService.toHttpParams(filters, sortParams)),
      )
      .subscribe(httpParams => this.paginatedLogs.updateHttpParams(httpParams));
  }
}
