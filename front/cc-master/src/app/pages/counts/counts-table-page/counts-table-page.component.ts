import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { BehaviorSubject, combineLatest, Observable, of, ReplaySubject, Subject } from 'rxjs';
import { map, skip, switchMap, take, takeUntil } from 'rxjs/operators';

import { CountAdditionalColumn } from './components/count-additional-column-select/count-additional-column.enum';
import { CountsSortParamKey } from './enums/count-sort-param-key.enum';
import { ICountsFilterForm } from './models/counts-filter-form.interface';
import { ICountsSummary } from './models/counts-summary.interface';
import { IDetailedCount } from './models/detailed-count.interface';
import { CountsApiMappingService } from './services/counts-api-mapping.service';
import { CountsDataService } from './services/counts-data.service';
import { CountsFilterRoutingService } from './services/counts-filter-routing.service';
import { CountsRoutingService } from './services/counts-routing.service';

@Component({
  selector: 'cc-counts-table-page',
  templateUrl: './counts-table-page.component.html',
  styleUrls: ['./counts-table-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsTablePageComponent implements OnInit, OnDestroy {

  public get counts$(): Observable<IDetailedCount[]> {
    return this.paginatedCounts.items$;
  }

  public get totalCount$(): Observable<number> {
    return this.paginatedCounts.totalCount$;
  }

  public get state$(): Observable<PaginatedListState> {
    return this.paginatedCounts.state$;
  }

  public summary$ = new ReplaySubject<ICountsSummary>(1);

  public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public filters: FormControl = new FormControl(null);
  public sortParams$: ReplaySubject<ISortParams> = new ReplaySubject<ISortParams>(1);

  public exportButtonClass$ = new ReplaySubject<string>(1);

  public columnsSelectedFormControl: FormControl = new FormControl();
  public columnsSelected$: ReplaySubject<CountAdditionalColumn[]> = new ReplaySubject(1);

  private paginatedCounts: PaginatedList<IDetailedCount>;
  private destroy$: Subject<void> = new Subject();

  constructor(
    private pagingService: PagingService,
    private dataService: CountsDataService,
    private apiMappingService: CountsApiMappingService,
    private routingService: CountsRoutingService,
    private filtersRoutingService: CountsFilterRoutingService,
  ) {
  }

  public ngOnInit(): void {
    combineLatest([this.filters.valueChanges, this.sortParams$])
      .pipe(
        takeUntil(this.destroy$),
        map(([filters, sort]) => ({ filters, sort })),
        map(attributes => this.apiMappingService.toHttpParams(attributes)),
      )
      .subscribe(httpParams => this.paginatedCounts.updateHttpParams(httpParams));

    this.filters.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        map(filters => this.apiMappingService.toHttpParams({ filters })),
        switchMap(httpParams => this.dataService.getSummary$(httpParams)))
      .subscribe(this.summary$);

    combineLatest([this.filters.valueChanges, this.columnsSelected$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(async ([filters, columns]) => await this.updateRoutingParamsAsync(filters, columns));

    this.columnsSelectedFormControl.valueChanges
      .pipe(map(columns => !!columns ? columns.map(c => c.id) : []))
      .subscribe(this.columnsSelected$);

    this.setFiltersAndColumnsAndSort();

    this.paginatedCounts = this.pagingService.paginate<IDetailedCount>(
      (httpParams) => this.getPaginatedCounts$(httpParams),
    );

    this.paginatedCounts.state$
      .pipe(skip(1), map(state => state === PaginatedListState.Update))
      .subscribe(this.isLoading$);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedCounts.nextPage();
  }

  public sort(sortParams: ISortParams) {
    this.sortParams$.next(sortParams);
  }

  public export(): void {
    const filters = this.filters.value;
    this.sortParams$
      .pipe(take(1), map(sort => this.apiMappingService.toHttpParams({ filters, sort })),
        switchMap(p => this.export$(p)))
      .subscribe(this.exportButtonClass$);
  }

  private export$(httpParams: HttpParams): Observable<string> {
    return this.dataService.export$(httpParams)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)));
  }

  private getPaginatedCounts$(httpParams: HttpParams): Observable<IPaginatedResult<IDetailedCount>> {
    return this.dataService.getDetailedCounts$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

  private async updateRoutingParamsAsync(filters: ICountsFilterForm, columns: CountAdditionalColumn[]): Promise<void> {
    const routingParams = this.filtersRoutingService.toRoutingParams(filters, columns);
    await this.routingService.updateRouterAsync(routingParams);
  }

  private setFiltersAndColumnsAndSort(): void {
    const routingParams = this.routingService.getRoutingParams();

    combineLatest([
      this.filtersRoutingService.toFilter$(routingParams),
      of(this.filtersRoutingService.toColumnsSelected(routingParams)),
      of({ field: CountsSortParamKey.CountPeriod, order: SortOrder.Desc }),
    ])
      .pipe(take(1))
      .subscribe(([filters, columns, sort]) => {
        this.filters.setValue(filters, { emitEvent: false });
        this.columnsSelectedFormControl.setValue(columns);
        this.sortParams$.next(sort);
      });
  }
}
