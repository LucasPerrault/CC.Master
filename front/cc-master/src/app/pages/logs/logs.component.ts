import {BehaviorSubject, combineLatest, Observable, Subject} from 'rxjs';
import {takeUntil} from 'rxjs/operators';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {IApiV3SortParams, IHttpQueryParams} from './queries';
import {LogsService} from './services';
import {IEnvironmentLog} from './models';

@Component({
  selector: 'cc-logs',
  templateUrl: './logs.component.html',
})
export class LogsComponent implements OnInit, OnDestroy {
  public defaultSortParams: IApiV3SortParams = {
    field: 'createdOn',
    order: 'desc'
  }
  private _queryFilterParams$: BehaviorSubject<IHttpQueryParams> = new BehaviorSubject<IHttpQueryParams>(null);
  private _sortParams$: BehaviorSubject<IApiV3SortParams> = new BehaviorSubject<IApiV3SortParams>(this.defaultSortParams);
  private _destroySubscription$: Subject<void> = new Subject<void>();

  constructor(private _logsService: LogsService) {}

  public ngOnInit(): void {
    this.refreshLogsWhenQueryParamsChange();
  }

  public ngOnDestroy(): void {
    this._destroySubscription$.next();
    this._destroySubscription$.complete();
  }

  public updateQueryFilters(queryFiltersParams: IHttpQueryParams): void {
    this._queryFilterParams$.next(queryFiltersParams);
  }

  public sortBy(sortParams: IApiV3SortParams) {
    this._sortParams$.next(sortParams);
  }

  public async showMoreDataAsync(): Promise<void> {
    await this._logsService.showMoreDataAsync(this._sortParams$.value, this._queryFilterParams$.value);
  }

  private refreshLogsWhenQueryParamsChange(): void {
    combineLatest([ this._queryFilterParams$, this._sortParams$])
      .pipe(takeUntil(this._destroySubscription$))
      .subscribe(async ([queryFilterParams, sortParams]) =>
        await this._logsService.refreshLogsAsync(sortParams, queryFilterParams)
      );
  }

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this._logsService.logs$;
  }
}
