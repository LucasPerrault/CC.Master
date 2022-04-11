import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, combineLatest, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, skip, startWith, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../common/components/advanced-filter-form';
import { FacetAndColumnHelper } from '../common/forms/select/facets-and-columns-api-select';
import { IAdditionalColumn, IFacet, ISearchDto, toSearchDto } from '../common/models';
import { IEstablishment } from '../common/models/establishment.interface';
import { AdvancedFilterColumnAutoSelection, ColumnAutoSelectionService } from '../common/services/column-auto-selection';
import { EstablishmentAdvancedFilterApiMappingService, EstablishmentAdvancedFilterConfiguration } from './advanced-filter';
import { EstablishmentAdditionalColumn, getAdditionalColumnByIds } from './models/establishment-additional-column';
import { etsAutoSelectedColumnMapping } from './models/establishment-auto-selected-column-mapping';
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

  public get submitDisabled(): boolean {
    return !this.filters.dirty || !this.filters.valid;
  }

  public get facets$(): Observable<IFacet[]> {
    return this.facetsAndColumns?.valueChanges.pipe(FacetAndColumnHelper.toFacets, startWith([]));
  }

  public exportButtonClass$ = new ReplaySubject<string>(1);

  public facetsAndColumns = new FormControl();
  public filters: FormControl = new FormControl();

  public facetColumns$ = new BehaviorSubject<IFacet[]>([]);
  public searchDto$ = new BehaviorSubject<ISearchDto>(null);
  public advancedFilter$ = new BehaviorSubject<AdvancedFilter>(null);
  public selectedColumns$ = new BehaviorSubject<IAdditionalColumn[]>(getAdditionalColumnByIds([
    EstablishmentAdditionalColumn.Name,
    EstablishmentAdditionalColumn.Environment,
    EstablishmentAdditionalColumn.Country,
  ]));

  private paginatedEts: PaginatedList<IEstablishment>;
  private columnAutoSelectionBuilder: AdvancedFilterColumnAutoSelection;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public advancedFilterConfig: EstablishmentAdvancedFilterConfiguration,
    private apiMappingService: EstablishmentAdvancedFilterApiMappingService,
    private pagingService: PagingService,
    private dataService: EstablishmentsDataService,
    private columnAutoSelectionService: ColumnAutoSelectionService,
  ) {
    this.paginatedEts = this.pagingService.paginate<IEstablishment>(
      (httpParams) => this.getEstablishments$(httpParams, this.searchDto$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );

    this.columnAutoSelectionBuilder = this.columnAutoSelectionService
      .create(this.filters, this.facetsAndColumns, etsAutoSelectedColumnMapping, this.destroy$);
  }

  public ngOnInit(): void {
    this.paginatedEts.state$
      .pipe(takeUntil(this.destroy$), skip(1), map(state => state === PaginatedListState.Update))
      .subscribe(isLoading => this.isLoading$.next(isLoading));

    combineLatest([this.advancedFilter$, this.facets$])
      .pipe(takeUntil(this.destroy$), map(([criterion, facets]) => toSearchDto(criterion, facets)))
      .subscribe(searchDto => this.searchDto$.next(searchDto));

    this.filters.valueChanges
      .pipe(takeUntil(this.destroy$), filter(() => !this.submitDisabled), this.toApiMapping)
      .subscribe(searchDto => this.advancedFilter$.next(searchDto));

    this.facetsAndColumns.setValue(FacetAndColumnHelper.mapColumnsToFacetAndColumns(this.selectedColumns$.value));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedEts.nextPage();
  }

  public submit(): void {
    this.facetColumns$.next(FacetAndColumnHelper.getFacets(this.facetsAndColumns.value));
    this.selectedColumns$.next(FacetAndColumnHelper.getColumns(this.facetsAndColumns.value));
    this.paginatedEts.updateHttpParams(new HttpParams());
  }

  public export(): void {
    this.dataService.export$(this.advancedFilter$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private getEstablishments$(httpParams: HttpParams, searchDto: ISearchDto): Observable<IPaginatedResult<IEstablishment>> {
    return this.dataService.getEstablishments$(httpParams, searchDto)
      .pipe(map(response => ({ items: response.items, totalCount: response.count })));
  }

  private get toApiMapping(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
