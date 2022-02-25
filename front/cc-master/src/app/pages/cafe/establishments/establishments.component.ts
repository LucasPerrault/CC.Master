import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, skip, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../common/components/advanced-filter-form';
import { IEstablishment } from '../common/models/establishment.interface';
import { EstablishmentAdvancedFilterApiMappingService, EstablishmentAdvancedFilterConfiguration } from './advanced-filter';
import { EstablishmentsDataService } from './services/establishments-data.service';

@Component({
  selector: 'cc-cafe-establishments',
  templateUrl: './establishments.component.html',
  styleUrls: ['./establishments.component.scss'],
})
export class EstablishmentsComponent implements OnInit, OnDestroy {
  public get establishments$(): Observable<IEstablishment[]> { return this.paginatedEts.items$; }
  public get totalCount$(): Observable<number> { return this.paginatedEts.totalCount$; }
  public isLoading$ = new ReplaySubject<boolean>(1);

  public get canExport(): boolean {
    return !!this.filters?.value?.criterionForms?.length;
  }

  public exportButtonClass$ = new ReplaySubject<string>(1);

  public filters: FormControl = new FormControl();
  public searchDto$ = new BehaviorSubject<AdvancedFilter>(null);

  private paginatedEts: PaginatedList<IEstablishment>;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public advancedFilterConfig: EstablishmentAdvancedFilterConfiguration,
    private apiMappingService: EstablishmentAdvancedFilterApiMappingService,
    private pagingService: PagingService,
    private dataService: EstablishmentsDataService,
  ) {
    this.paginatedEts = this.pagingService.paginate<IEstablishment>(
      (httpParams) => this.getEstablishments$(httpParams, this.searchDto$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  public ngOnInit(): void {
    this.paginatedEts.state$
      .pipe(takeUntil(this.destroy$), skip(1), map(state => state === PaginatedListState.Update))
      .subscribe(isLoading => this.isLoading$.next(isLoading));

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
    this.paginatedEts.nextPage();
  }

  public export(): void {
    this.dataService.export$(this.searchDto$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private refresh(): void {
    this.paginatedEts.updateHttpParams(new HttpParams());
  }

  private getEstablishments$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IEstablishment>> {
    return this.dataService.getEstablishments$(httpParams, advancedFilter)
      .pipe(map(response => ({ items: response.items, totalCount: response.count })));
  }

  private get toApiMapping(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
