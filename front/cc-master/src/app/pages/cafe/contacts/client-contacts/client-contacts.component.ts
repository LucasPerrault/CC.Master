import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../../common/cafe-filters/advanced-filter-form';
import { ClientContactAdvancedFilterApiMappingService } from './advanced-filter/client-contact-advanced-filter-api-mapping.service';
import { IClientContact } from './client-contact.interface';
import { clientContactAdditionalColumns } from './client-contact-additional-column.enum';
import { ClientContactsDataService } from './client-contacts-data.service';

@Component({
  selector: 'cc-client-contacts',
  templateUrl: './client-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientContactsComponent implements OnInit, OnDestroy {
  @Input() public set advancedFilterForm(f: IAdvancedFilterForm) { this.setAdvancedFilter(f); }

  public get contacts$(): Observable<IClientContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public advancedFilter$: BehaviorSubject<AdvancedFilter> = new BehaviorSubject<AdvancedFilter>(null);

  public selectedColumns: FormControl = new FormControl([]);
  public additionalColumns = clientContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IClientContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
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
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private refresh(): void {
    this.paginatedContacts.updateHttpParams(new HttpParams());
  }

  private getPaginatedClientContacts$(): PaginatedList<IClientContact> {
    return this.pagingService.paginate<IClientContact>(
      (httpParams) => this.getClientContacts$(httpParams, this.advancedFilter$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getClientContacts$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<IClientContact>> {
    return this.contactsService.getClientContacts$(httpParams, advancedFilter)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }

  private setAdvancedFilter(form: IAdvancedFilterForm) {
    const advancedFilter = this.apiMappingService.toAdvancedFilter(form);
    this.advancedFilter$.next(advancedFilter);
  }
}
