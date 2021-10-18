import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IClientContact } from './client-contact.interface';
import { clientContactAdditionalColumns } from './client-contact-additional-column.enum';
import { ClientContactsDataService } from './client-contacts-data.service';

@Component({
  selector: 'cc-client-contacts',
  templateUrl: './client-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientContactsComponent {
  public get contacts$(): Observable<IClientContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);
  public additionalColumns = clientContactAdditionalColumns;
  private paginatedContacts: PaginatedList<IClientContact>;

  constructor(
    private pagingService: PagingService,
    private contactsService: ClientContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedClientContacts$();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private getPaginatedClientContacts$(): PaginatedList<IClientContact> {
    return this.pagingService.paginate<IClientContact>(
      (httpParams) => this.getClientContacts$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getClientContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IClientContact>> {
    return this.contactsService.getClientContacts$(httpParams)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }
}
