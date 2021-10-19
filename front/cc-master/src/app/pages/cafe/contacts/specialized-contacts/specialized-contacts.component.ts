import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterForm } from '../../common/cafe-filters/advanced-filter-form';
import {
  SpecializedContactAdvancedFilterApiMappingService,
} from './advanced-filter/specialized-contact-advanced-filter-api-mapping.service';
import { ISpecializedContact } from './specialized-contact.interface';
import { specializedContactAdditionalColumns } from './specialized-contact-additional-column.enum';
import { SpecializedContactsDataService } from './specialized-contacts-data.service';


@Component({
  selector: 'cc-specialized-contacts',
  templateUrl: './specialized-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./specialized-contacts.component.scss'],
})
export class SpecializedContactsComponent implements OnInit, OnDestroy {
  @Input() public set advancedFilterForm(f: IAdvancedFilterForm) { this.setAdvancedFilter(f); }

  public get contacts$(): Observable<ISpecializedContact[]> {
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
  public additionalColumns = specializedContactAdditionalColumns;
  private paginatedContacts: PaginatedList<ISpecializedContact>;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private pagingService: PagingService,
    private apiMappingService: SpecializedContactAdvancedFilterApiMappingService,
    private contactsService: SpecializedContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedSpeContacts$();
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


  private getPaginatedSpeContacts$(): PaginatedList<ISpecializedContact> {
    return this.pagingService.paginate<ISpecializedContact>(
      (httpParams) => this.getSpeContacts$(httpParams, this.advancedFilter$.value),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getSpeContacts$(httpParams: HttpParams, advancedFilter: AdvancedFilter): Observable<IPaginatedResult<ISpecializedContact>> {
    return this.contactsService.getSpecializedContacts$(httpParams, advancedFilter)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }

  private setAdvancedFilter(form: IAdvancedFilterForm) {
    const advancedFilter = this.apiMappingService.toAdvancedFilter(form);
    this.advancedFilter$.next(advancedFilter);
  }
}
