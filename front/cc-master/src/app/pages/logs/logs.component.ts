import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { apiV3SortKey, apiV3SortToHttpParams, toApiDateRangeV3Format, toApiV3SortParams } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { IEnvironmentLog, LogsService } from '@cc/domain/environments';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

import { ILogsFilter } from './models/logs-filter.interface';

enum EnvironmentLogQueryParamKey {
  UserId = 'userId',
  ActivityId = 'activityId',
  EnvironmentDomain = 'environment.domain',
  EnvironmentId = 'environmentId',
  CreatedOn = 'createdOn',
  IsAnonymizedData = 'isAnonymizedData'
}

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsComponent implements OnInit, OnDestroy {
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

  constructor(private logsService: LogsService, private pagingService: PagingService) {
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

  public updateFilters(filters: ILogsFilter): void {
    const httpParams = this.toHttpParams(filters);
    this.paginatedLogs.updateFilters(httpParams);
  }

  public updateSort(sortParams: ISortParams[]) {
    const apiV3SortParams = toApiV3SortParams(sortParams);
    this.paginatedLogs.updateSort(apiV3SortParams);
  }

  public showMore(): void {
    this.paginatedLogs.showMore();
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

  private toHttpParams(filters: ILogsFilter): HttpParams {
    let params = new HttpParams();
    if (!!filters.environmentIds.length) {
      params = params.set(EnvironmentLogQueryParamKey.EnvironmentId, filters.environmentIds.join(','));
    }

    if (!!filters.users.length) {
      const userIds = filters.users.map(u => u.id);
      params = params.set(EnvironmentLogQueryParamKey.UserId, userIds.join(','));
    }

    if (!!filters.isAnonymizedData) {
      params = params.set(EnvironmentLogQueryParamKey.IsAnonymizedData, filters.isAnonymizedData);
    }

    const createdOn = toApiDateRangeV3Format(filters.createdOn);
    if (!!createdOn) {
      params = params.set(EnvironmentLogQueryParamKey.CreatedOn, createdOn);
    }

    if (!!filters.actionIds.length) {
      params = params.set(EnvironmentLogQueryParamKey.ActivityId, filters.actionIds.join(','));
    }

    if (!!filters.domainIds.length) {
      params = params.set(EnvironmentLogQueryParamKey.EnvironmentDomain, filters.domainIds.join(','));
    }

    return params;
  }
}
