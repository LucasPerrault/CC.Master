import { HttpParams } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService } from './advanced-filter';
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

  public selectedColumns: FormControl = new FormControl([]);

  private paginatedEnvironments: PaginatedList<IEnvironment>;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private pagingService: PagingService,
    private apiMappingService: EnvironmentAdvancedFilterApiMappingService,
    private environmentsDataService: EnvironmentDataService,
  ) {
    this.paginatedEnvironments = this.getPaginatedEnvironments$();
  }

  public ngOnInit(): void {
    this.advancedFilter$
      .pipe(takeUntil(this.destroy$), filter(f => !!f))
      .subscribe(() => this.refresh());
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
