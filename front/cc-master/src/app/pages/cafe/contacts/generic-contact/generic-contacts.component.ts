import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IGenericContact } from './generic-contact.interface';
import { GenericContactsDataService } from './generic-contacts-data.service';
import { genericContactAdditionalColumns } from './generic-contact-additional-column.enum';

@Component({
  selector: 'cc-generic-contacts',
  templateUrl: './generic-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GenericContactsComponent {
  public get contacts$(): Observable<IGenericContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);
  public additionalColumns = genericContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IGenericContact>;

  constructor(
    private pagingService: PagingService,
    private contactsService: GenericContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedGenericContacts$();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private getPaginatedGenericContacts$(): PaginatedList<IGenericContact> {
    return this.pagingService.paginate<IGenericContact>(
      (httpParams) => this.getGenericContacts$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getGenericContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IGenericContact>> {
    return this.contactsService.getContacts$(httpParams)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }
}
