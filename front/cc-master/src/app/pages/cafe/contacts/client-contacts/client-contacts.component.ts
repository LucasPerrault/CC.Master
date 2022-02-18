import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { BehaviorSubject, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, take, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../../common/components/advanced-filter-form';
import { ClientContactAdvancedFilterConfiguration } from './advanced-filter/client-contact-advanced-filter.configuration';
import { ClientContactAdvancedFilterApiMappingService } from './advanced-filter/client-contact-advanced-filter-api-mapping.service';
import { IClientContact } from './client-contact.interface';
import {
  ClientContactAdditionalColumn,
  clientContactAdditionalColumns,
  getAdditionalColumnByIds,
} from './client-contact-additional-column.enum';
import { ClientContactsDataService } from './client-contacts-data.service';

@Component({
  selector: 'cc-client-contacts',
  templateUrl: './client-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./client-contacts.component.scss'],
})
export class ClientContactsComponent implements OnInit, OnDestroy {
  public get contacts$(): Observable<IClientContact[]> {
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
    ClientContactAdditionalColumn.Environment,
    ClientContactAdditionalColumn.LastName,
    ClientContactAdditionalColumn.FirstName,
    ClientContactAdditionalColumn.Mail,
    ClientContactAdditionalColumn.Client,
  ]));
  public additionalColumns = clientContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IClientContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    public configuration: ClientContactAdvancedFilterConfiguration,
    private pagingService: PagingService,
    private apiMappingService: ClientContactAdvancedFilterApiMappingService,
    private contactsService: ClientContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedClientContacts$();
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
    this.contactsService.exportClientContacts$(this.advancedFilter$.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(c => this.exportButtonClass$.next(c));
  }

  private refresh(): void {
    this.paginatedContacts.updateHttpParams(new HttpParams());
  }

  private getPaginatedClientContacts$(): PaginatedList<IClientContact> {
    return this.pagingService.paginate<IClientContact>(
      (httpParams) => this.getClientContacts$(httpParams, this.advancedFilter$.value),
      { page: defaultPagingParams.page, limit: 50 },
      ApiStandard.V4,
    );
  }

  private getClientContacts$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IClientContact>> {
    return this.contactsService.getClientContacts$(httpParams, advancedFilter)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }

  private get toAdvancedFilter(): UnaryFunction<Observable<IAdvancedFilterForm>, Observable<AdvancedFilter>> {
    return pipe(map(filters => this.apiMappingService.toAdvancedFilter(filters)));
  }
}
