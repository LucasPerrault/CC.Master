import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { apiV3SortKey, apiV3SortToHttpParams, toApiV3SortParams } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { IEnvironmentLog, LogsService } from '@cc/domain/environments';
import { Observable, Subject } from 'rxjs';
import { map, take } from 'rxjs/operators';

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
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public async updateFiltersAsync(filters: ILogsFilter): Promise<void> {
    const httpParams = this.logsApiMappingService.toHttpParams(filters);
    const routingParams = this.logsFilterRoutingService.toLogsRoutingParams(filters);

    this.paginatedLogs.updateFilters(httpParams);
    await this.logsRoutingService.updateRouterAsync(routingParams);
  }

  public updateSort(sortParams: ISortParams) {
    const apiV3SortParams = toApiV3SortParams([sortParams]);
    this.paginatedLogs.updateSort(apiV3SortParams);
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
    const params = this.getDefaultSortHttpParams(httpParams);
    return this.logsService.getLogs$(params).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

  private getDefaultSortHttpParams(httpParams: HttpParams): HttpParams {
    const isAlreadySorted = httpParams.has(apiV3SortKey);
    if (isAlreadySorted) {
      return httpParams;
    }

    const apiV3DefaultSortParams = toApiV3SortParams([this.defaultSortParams]);
    return apiV3SortToHttpParams(httpParams, apiV3DefaultSortParams);
  }
}
