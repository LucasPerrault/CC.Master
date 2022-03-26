import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { BehaviorSubject, combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, } from '../common/components/advanced-filter-form';
import {
  EnvironmentAdditionalColumn,
  environmentCriterionAndColumnMapping,
  FacetAndColumnHelper,
  getAdditionalColumnByIds,
} from '../common/forms/select/facets-and-columns-api-select';
import {
  ColumnAutoSelectionOrchestrator,
  CriterionSelectionState,
  IColumnAutoSelectionOrchestratorData,
} from '../common/forms/select/facets-and-columns-api-select/environment/column-auto-selection-orchestrator';
import { IAdditionalColumn, IFacet, ISearchDto, toSearchDto } from '../common/models';
import { IEnvironment } from '../common/models/environment.interface';
import { EnvironmentAdvancedFilterApiMappingService, EnvironmentAdvancedFilterConfiguration } from './advanced-filter';
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

  public get submitDisabled(): boolean {
    return !this.advancedFilter.dirty || !this.advancedFilter.valid;
  }

  public get facets$(): Observable<IFacet[]> {
    return this.facetsAndColumns.valueChanges.pipe(FacetAndColumnHelper.toFacets);
  }

  public exportButtonClass$ = new ReplaySubject<string>(1);

  public advancedFilter = new FormControl({ criterionForms: [] });
  public facetsAndColumns = new FormControl();

  public selectedColumns$ = new BehaviorSubject<IAdditionalColumn[]>([]);
  public facetColumns$ = new BehaviorSubject<IFacet[]>([]);
  private advancedFilter$ = new BehaviorSubject<AdvancedFilter>(null);
  private searchDto$ = new BehaviorSubject<ISearchDto>(null);
  private autoSelectionOrchestrator = new ColumnAutoSelectionOrchestrator(this.advancedFilter.valueChanges);

  private paginatedEnvironments: PaginatedList<IEnvironment>;

  private defaultSelectedColumns = getAdditionalColumnByIds([
    EnvironmentAdditionalColumn.Environment,
    EnvironmentAdditionalColumn.AppInstances,
    EnvironmentAdditionalColumn.Distributors,
  ]);
  private addedCriterion$ = new BehaviorSubject(null);
  private removedCriterion$ = new BehaviorSubject(null);

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
    combineLatest([this.advancedFilter$, this.facets$])
      .pipe(takeUntil(this.destroy$), map(([criterion, facets]) => toSearchDto(criterion, facets)))
      .subscribe(searchDto => this.searchDto$.next(searchDto));

    this.advancedFilter.valueChanges
      .pipe(takeUntil(this.destroy$), filter(() => !this.submitDisabled), map(f => this.apiMappingService.toAdvancedFilter(f)))
      .subscribe(advancedFilter => this.advancedFilter$.next(advancedFilter));

    this.facetsAndColumns.setValue(FacetAndColumnHelper.mapColumnsToFacetAndColumns(this.defaultSelectedColumns));

    this.autoSelectionOrchestrator.valueChanges$
      .pipe(takeUntil(this.destroy$))
      .subscribe(d => this.updateFacetAndColumns(d));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submit(): void {
    this.facetColumns$.next(FacetAndColumnHelper.getFacets(this.facetsAndColumns.value));
    this.selectedColumns$.next(FacetAndColumnHelper.getColumns(this.facetsAndColumns.value));

    this.paginatedEnvironments.updateHttpParams(new HttpParams());
  }

  public nextPage(): void {
    this.paginatedEnvironments.nextPage();
  }

  public export(): void {
    this.environmentsDataService.exportEnvironments$(this.advancedFilter$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private getPaginatedEnvironments$(): PaginatedList<IEnvironment> {
    return this.pagingService.paginate<IEnvironment>(
      (httpParams) => this.getEnvironments$(httpParams, this.searchDto$.value),
      { page: defaultPagingParams.page, limit: 50 },
      ApiStandard.V4,
    );
  }

  private getEnvironments$(httpParams: HttpParams, searchDto: ISearchDto): Observable<IPaginatedResult<IEnvironment>> {
    return this.environmentsDataService.getEnvironments$(httpParams, searchDto).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }
  //
  // private getCriterionKeys(f: IAdvancedFilterForm) {
  //   return f.criterionForms.map(form =>
  //     this.isFacet(form) ? `${ (form.criterion as IFacetComparisonCriterion).facet.applicationId } - ${ }`)
  // }
  //
  // private isFacet(form: IComparisonFilterCriterionForm | IFacetComparisonCriterion): form is IFacetComparisonCriterion {
  //   return !!(form as IFacetComparisonCriterion).facet;
  // }
  private updateFacetAndColumns(selection: IColumnAutoSelectionOrchestratorData) {
    console.log(selection)
    const selectedKey = selection?.criterion?.key;
    const mapping = environmentCriterionAndColumnMapping.find(e => e.criterionKey === selectedKey);

    if (selection.state === CriterionSelectionState.Idle || !mapping?.column) {
      return;
    }

    const isAlreadySelected = this.facetsAndColumns.value.find(c => c.id === selectedKey);
    if (selection.state === CriterionSelectionState.WasAdded && !isAlreadySelected) {
      this.facetsAndColumns.patchValue([
        ...this.facetsAndColumns.value,
        FacetAndColumnHelper.getFacetAndColumn(mapping.column),
      ]);
    }

    if (selection.state === CriterionSelectionState.WasRemoved) {
      const values = this.facetsAndColumns.value.filter(v => v.id !== selectedKey);
      this.facetsAndColumns.patchValue(values);
    }
  }
}
