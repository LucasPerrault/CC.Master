import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import {
  AdvancedFilter,
  IAdvancedFilterForm,
} from '../../common/cafe-filters/advanced-filter-form';
import { AppContactAdvancedFilterConfiguration } from './advanced-filter/app-contact-advanced-filter.configuration';
import { AppContactAdvancedFilterApiMappingService } from './advanced-filter/app-contact-advanced-filter-api-mapping.service';
import { IAppContact } from './app-contact.interface';
import { appContactAdditionalColumns } from './app-contact-additional-column.enum';
import { AppContactsDataService } from './app-contacts-data.service';

@Component({
  selector: 'cc-app-contacts',
  templateUrl: './app-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppContactsComponent implements OnInit, OnDestroy {
  @Input() public set advancedFilterForm(f: IAdvancedFilterForm) { this.setAdvancedFilter(f); }
  public get contacts$(): Observable<IAppContact[]> {
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
  public additionalColumns = appContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IAppContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private configuration: AppContactAdvancedFilterConfiguration,
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

  private setAdvancedFilter(form: IAdvancedFilterForm) {
    const advancedFilter = this.apiMappingService.toAdvancedFilter(form);
    this.advancedFilter$.next(advancedFilter);
  }
}
