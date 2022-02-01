import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../../common/components/advanced-filter-form';
import { SpecializedContactAdvancedFilterConfiguration } from './advanced-filter/specialized-contact-advanced-filter.configuration';
import {
  SpecializedContactAdvancedFilterApiMappingService,
} from './advanced-filter/specialized-contact-advanced-filter-api-mapping.service';
import { ISpecializedContact } from './specialized-contact.interface';
import {
  getAdditionalColumnByIds,
  SpecializedContactAdditionalColumn,
  specializedContactAdditionalColumns,
} from './specialized-contact-additional-column.enum';
import { SpecializedContactsDataService } from './specialized-contacts-data.service';


@Component({
  selector: 'cc-specialized-contacts',
  templateUrl: './specialized-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./specialized-contacts.component.scss'],
})
export class SpecializedContactsComponent implements OnInit, OnDestroy {
  public get contacts$(): Observable<ISpecializedContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public get canExport(): boolean {
    return !!this.filters?.value?.criterionForms?.length;
  }

  public filters: FormControl = new FormControl();
  public searchDto$ = new BehaviorSubject<AdvancedFilter>(null);
  public exportButtonClass$ = new ReplaySubject<string>(1);

  public selectedColumns: FormControl = new FormControl(getAdditionalColumnByIds([
    SpecializedContactAdditionalColumn.Environment,
    SpecializedContactAdditionalColumn.LastName,
    SpecializedContactAdditionalColumn.FirstName,
    SpecializedContactAdditionalColumn.Mail,
    SpecializedContactAdditionalColumn.Role,
  ]));

  public additionalColumns = specializedContactAdditionalColumns;
  private paginatedContacts: PaginatedList<ISpecializedContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public configuration: SpecializedContactAdvancedFilterConfiguration,
    private pagingService: PagingService,
    private apiMappingService: SpecializedContactAdvancedFilterApiMappingService,
    private contactsService: SpecializedContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedSpeContacts$();
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
    this.paginatedContacts.nextPage();
  }

  public export(): void {
    this.contactsService.exportSpeContacts$(this.searchDto$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private refresh(): void {
    this.paginatedContacts.updateHttpParams(new HttpParams());
  }

  private getPaginatedSpeContacts$(): PaginatedList<ISpecializedContact> {
    return this.pagingService.paginate<ISpecializedContact>(
      (httpParams) => this.getSpeContacts$(httpParams, this.searchDto$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getSpeContacts$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<ISpecializedContact>> {
    return this.contactsService.getSpecializedContacts$(httpParams, advancedFilter)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }

  private get toApiMapping(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
