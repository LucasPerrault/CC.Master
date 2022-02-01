import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../common/components/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService, EnvironmentAdvancedFilterConfiguration } from './advanced-filter';
import {
  EnvironmentAdditionalColumn,
  getAdditionalColumnByIds,
} from './components/environment-additional-column-select/environment-additional-column.enum';
import { IEnvironment } from './models/environment.interface';
import { EnvironmentDataService } from './services/environment-data.service';

@Component({
  selector: 'cc-cafe-environments',
  templateUrl: './cafe-environments.component.html',
  styleUrls: ['./cafe-environments.component.scss'],
})
export class CafeEnvironmentsComponent implements OnInit, OnDestroy {
  public get environments$(): Observable<IEnvironment[]> {
    return this.paginatedEnvironments.items$;
  }

  public get environmentsCount$(): Observable<number> {
    return this.paginatedEnvironments.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedEnvironments.state$
      .pipe(map(state => state === PaginatedListState.Update));
  }

  public get canExport(): boolean {
    return !!this.filters?.value?.criterionForms?.length;
  }

  public exportButtonClass$ = new ReplaySubject<string>(1);
  public selectedColumns: FormControl = new FormControl(getAdditionalColumnByIds([
    EnvironmentAdditionalColumn.Environment,
    EnvironmentAdditionalColumn.AppInstances,
    EnvironmentAdditionalColumn.Distributors,
  ]));

  public filters: FormControl = new FormControl();
  public searchDto$ = new BehaviorSubject<AdvancedFilter>(null);

  private paginatedEnvironments: PaginatedList<IEnvironment>;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public advancedFilterConfig: EnvironmentAdvancedFilterConfiguration,
    private pagingService: PagingService,
    private apiMappingService: EnvironmentAdvancedFilterApiMappingService,
    private environmentsDataService: EnvironmentDataService,
  ) {
    this.paginatedEnvironments = this.getPaginatedEnvironments$();
  }

  public ngOnInit(): void {
    this.searchDto$
      .pipe(takeUntil(this.destroy$), filter(f => !!f))
      .subscribe(() => this.refresh());

    this.filters.valueChanges
      .pipe(takeUntil(this.destroy$), this.toApiMapping)
      .subscribe(searchDto => this.searchDto$.next(searchDto));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedEnvironments.nextPage();
  }

  public export(): void {
    this.environmentsDataService.exportEnvironments$(this.searchDto$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private refresh(): void {
    this.paginatedEnvironments.updateHttpParams(new HttpParams());
  }

  private getPaginatedEnvironments$(): PaginatedList<IEnvironment> {
    return this.pagingService.paginate<IEnvironment>(
      (httpParams) => this.getEnvironments$(httpParams, this.searchDto$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getEnvironments$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IEnvironment>> {
    return this.environmentsDataService.getEnvironments$(httpParams, advancedFilter).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

  private get toApiMapping(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
