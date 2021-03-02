import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { apiV3SortToHttpParams, toApiV3SortParams } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { IEnvironmentLog, LogsService } from '@cc/domain/environments';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
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
  public logsFilter: ILogsFilter;

  public defaultSortParams: ISortParams = {
    field: 'createdOn',
    order: SortOrder.Desc,
  };

  private httpParams: BehaviorSubject<HttpParams> = new BehaviorSubject<HttpParams>(new HttpParams());
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
    this.initLogsFilterWithRoutingParams();
  }

  public ngOnInit(): void {
    this.paginatedLogs = this.pagingService.paginate<IEnvironmentLog>(
      (httpParams) => this.getPaginatedLogs$(httpParams),
    );

    this.httpParams
      .pipe(takeUntil(this.destroySubscription$), debounceTime(300))
      .subscribe(params => this.paginatedLogs.updateHttpParams(params));
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public async updateFiltersAsync(filters: ILogsFilter): Promise<void> {
    const httpParamsWithFilters = this.logsApiMappingService.toHttpParams(filters, this.httpParams.value);
    this.httpParams.next(httpParamsWithFilters);

    const routingParams = this.logsFilterRoutingService.toLogsRoutingParams(filters);
    await this.logsRoutingService.updateRouterAsync(routingParams);
  }

  public updateSort(sortParams: ISortParams) {
    const apiV3SortParams = toApiV3SortParams(sortParams);
    const httpParamsWithSort = apiV3SortToHttpParams(this.httpParams.value, apiV3SortParams);
    this.httpParams.next(httpParamsWithSort);
  }

  public showMore(): void {
    this.paginatedLogs.showMore();
  }

  private initLogsFilterWithRoutingParams(): void {
    const routingParams = this.logsRoutingService.getLogsRoutingParams();
    this.logsFilterRoutingService.toLogsFilter$(routingParams)
      .pipe(take(1))
      .subscribe(f => this.logsFilter = f);
  }

  private getPaginatedLogs$(httpParams: HttpParams): Observable<IPaginatedResult<IEnvironmentLog>> {
    return this.logsService.getLogs$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }
}
