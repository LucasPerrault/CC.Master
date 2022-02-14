import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { toSubmissionState } from '@cc/common/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { filter, map, switchMap, take, takeUntil } from 'rxjs/operators';

import { CafeExportService } from '../../cafe-export.service';
import {
  AdvancedFilter,
  IAdvancedFilterForm,
} from '../../common/cafe-filters/advanced-filter-form';
import { AppContactAdvancedFilterConfiguration } from './advanced-filter/app-contact-advanced-filter.configuration';
import { AppContactAdvancedFilterApiMappingService } from './advanced-filter/app-contact-advanced-filter-api-mapping.service';
import { IAppContact } from './app-contact.interface';
import { AppContactAdditionalColumn, appContactAdditionalColumns, getAdditionalColumnByIds } from './app-contact-additional-column.enum';
import { AppContactsDataService } from './app-contacts-data.service';
import { ApiStandard } from '@cc/common/queries';

@Component({
  selector: 'cc-app-contacts',
  templateUrl: './app-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./app-contacts.component.scss'],
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
    private configuration: AppContactAdvancedFilterConfiguration,
    private apiMappingService: AppContactAdvancedFilterApiMappingService,
    private pagingService: PagingService,
		private contactsService: AppContactsDataService,
		private exportService: CafeExportService,
  ) {
    this.paginatedContacts = this.getPaginatedAppContacts$();
  }

  public ngOnInit(): void {
    this.advancedFilter$
      .pipe(takeUntil(this.destroy$), filter(f => !!f))
			.subscribe(() => this.refresh());

		this.exportService.exportRequests$
			.pipe(
				takeUntil(this.destroy$),
        filter(() => !!this.advancedFilter$.value),
        map(() => this.advancedFilter$.value),
        switchMap(f => this.contactsService.exportAppContacts$(f).pipe(take(1), toSubmissionState())),
      )
      .subscribe(s => this.exportService.notifyExport(s));
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
      ApiStandard.V4,
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
