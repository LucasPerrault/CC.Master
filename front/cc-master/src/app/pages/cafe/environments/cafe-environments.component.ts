import { HttpParams } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, switchMap, take, takeUntil } from 'rxjs/operators';

import { CafeExportService } from '../cafe-export.service';
import { AdvancedFilter, IAdvancedFilterForm } from '../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService } from './advanced-filter';
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
  @Input() public set advancedFilterForm(f: IAdvancedFilterForm) { this.setAdvancedFilter(f); }

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

  public advancedFilter$: BehaviorSubject<AdvancedFilter> = new BehaviorSubject<AdvancedFilter>(null);

  public selectedColumns: FormControl = new FormControl(getAdditionalColumnByIds([
    EnvironmentAdditionalColumn.Environment,
    EnvironmentAdditionalColumn.AppInstances,
    EnvironmentAdditionalColumn.Distributors,
  ]));

  private paginatedEnvironments: PaginatedList<IEnvironment>;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private pagingService: PagingService,
    private apiMappingService: EnvironmentAdvancedFilterApiMappingService,
    private environmentsDataService: EnvironmentDataService,
    private exportService: CafeExportService,
  ) {
    this.paginatedEnvironments = this.getPaginatedEnvironments$();
  }

  public ngOnInit(): void {
    this.advancedFilter$
      .pipe(takeUntil(this.destroy$), filter(f => !!f))
      .subscribe(() => this.refresh());

    this.exportService.exportRequests$
      .pipe(
        takeUntil(this.destroy$),
        filter(() => !!this.advancedFilter$.value),
        map(() => this.advancedFilter$.value),
        switchMap(f => this.environmentsDataService.exportEnvironments$(f).pipe(take(1), toSubmissionState())),
      )
      .subscribe(s => this.exportService.notifyExport(s));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedEnvironments.nextPage();
  }

  private refresh(): void {
    this.paginatedEnvironments.updateHttpParams(new HttpParams());
  }

  private getPaginatedEnvironments$(): PaginatedList<IEnvironment> {
    return this.pagingService.paginate<IEnvironment>(
      (httpParams) => this.getEnvironments$(httpParams, this.advancedFilter$.value),
      { page: defaultPagingParams.page, limit: 50 },
      ApiStandard.V4,
    );
  }

  private getEnvironments$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IEnvironment>> {
    return this.environmentsDataService.getEnvironments$(httpParams, advancedFilter).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

  private setAdvancedFilter(form: IAdvancedFilterForm) {
    const advancedFilter = this.apiMappingService.toAdvancedFilter(form);
    this.advancedFilter$.next(advancedFilter);
  }
}
