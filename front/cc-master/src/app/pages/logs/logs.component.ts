import { Component, OnDestroy, OnInit } from '@angular/core';
import { IFilterParams } from '@cc/common/filter';
import { IPaginatedResult, IPagingParams, PaginatedList, PagingService } from '@cc/common/paging';
import { ISortParams } from '@cc/common/sort';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

import { LogsService } from '../../domain/environments';
import { IEnvironmentLog } from './models';

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
})
export class LogsComponent implements OnInit, OnDestroy {
  public defaultSortParams: ISortParams = {
    field: 'createdOn',
    order: 'desc',
  };

  private destroySubscription$: Subject<void> = new Subject<void>();
  private paginatedLogs: PaginatedList<IEnvironmentLog>;

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this.paginatedLogs.items$;
  }

  public get isLoadMore$(): Observable<boolean> {
    return this.paginatedLogs.isLoadMore$;
  }

  public get isUpdateData$(): Observable<boolean> {
    return this.paginatedLogs.isUpdateData$;
  }

  constructor(private logsService: LogsService, private pagingService: PagingService) {
  }

  public ngOnInit(): void {
    this.paginatedLogs = this.pagingService.paginate<IEnvironmentLog>(
      (paging, sort, filter) => this.getPaginatedLogs$(paging, sort, filter),
    );
    this.paginatedLogs.updateSort(this.defaultSortParams);
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public updateFilters(queryFiltersParams: IFilterParams): void {
    this.paginatedLogs.updateFilters(queryFiltersParams);
  }

  public updateSort(sortParams: ISortParams) {
    this.paginatedLogs.updateSort(sortParams);
  }

  public showMore(): void {
    this.paginatedLogs.showMore();
  }

  private getPaginatedLogs$(
    paging: IPagingParams,
    sort: ISortParams,
    filter: IFilterParams,
  ): Observable<IPaginatedResult<IEnvironmentLog>> {
    return this.logsService.getLogs$(paging, sort, filter).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }
}
