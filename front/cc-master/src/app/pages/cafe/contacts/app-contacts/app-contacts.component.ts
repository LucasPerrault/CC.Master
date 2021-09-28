import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IAppContact } from './app-contact.interface';
import { AppContactsDataService } from './app-contacts-data.service';

@Component({
  selector: 'cc-app-contacts',
  templateUrl: './app-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppContactsComponent {
  public get contacts$(): Observable<IAppContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);
  private paginatedContacts: PaginatedList<IAppContact>;

  constructor(
    private pagingService: PagingService,
    private contactsService: AppContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedAppContacts$();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private getPaginatedAppContacts$(): PaginatedList<IAppContact> {
    return this.pagingService.paginate<IAppContact>(
      (httpParams) => this.getAppContacts$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getAppContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IAppContact>> {
    return this.contactsService.getAppContacts$(httpParams)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }
}
