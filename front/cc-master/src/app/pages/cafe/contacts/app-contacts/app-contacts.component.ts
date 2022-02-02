import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import {
  AdvancedFilter,
  IAdvancedFilterForm,
} from '../../common/components/advanced-filter-form';
import { AppContactAdvancedFilterConfiguration } from './advanced-filter/app-contact-advanced-filter.configuration';
import { AppContactAdvancedFilterApiMappingService } from './advanced-filter/app-contact-advanced-filter-api-mapping.service';
import { IAppContact } from './app-contact.interface';
import { AppContactAdditionalColumn, appContactAdditionalColumns, getAdditionalColumnByIds } from './app-contact-additional-column.enum';
import { AppContactsDataService } from './app-contacts-data.service';

@Component({
  selector: 'cc-app-contacts',
  templateUrl: './app-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./app-contacts.component.scss'],
})
export class AppContactsComponent implements OnInit, OnDestroy {
  public get contacts$(): Observable<IAppContact[]> {
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
  public advancedFilter$ = new BehaviorSubject<AdvancedFilter>(null);
  public exportButtonClass$ = new ReplaySubject<string>(1);

  public selectedColumns: FormControl = new FormControl(getAdditionalColumnByIds([
    AppContactAdditionalColumn.Environment,
    AppContactAdditionalColumn.LastName,
    AppContactAdditionalColumn.FirstName,
    AppContactAdditionalColumn.AppInstance,
    AppContactAdditionalColumn.Mail,
    AppContactAdditionalColumn.IsConfirmed,
  ]));
  public additionalColumns = appContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IAppContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public configuration: AppContactAdvancedFilterConfiguration,
    private apiMappingService: AppContactAdvancedFilterApiMappingService,
    private pagingService: PagingService,
		private contactsService: AppContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedAppContacts$();
  }

  public ngOnInit(): void {
    this.advancedFilter$
      .pipe(takeUntil(this.destroy$), filter(f => !!f))
      .subscribe(() => this.refresh());

    this.filters.valueChanges
      .pipe(takeUntil(this.destroy$), this.toAdvancedFilter)
      .subscribe(advancedFilter => this.advancedFilter$.next(advancedFilter));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  public export(): void {
    this.contactsService.exportAppContacts$(this.advancedFilter$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private refresh(): void {
    this.paginatedContacts.updateHttpParams(new HttpParams());
  }

  private getPaginatedAppContacts$(): PaginatedList<IAppContact> {
    return this.pagingService.paginate<IAppContact>(
      (httpParams) => this.getAppContacts$(httpParams, this.advancedFilter$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getAppContacts$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IAppContact>> {
    return this.contactsService.getAppContacts$(httpParams, advancedFilter)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }

  private get toAdvancedFilter(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
