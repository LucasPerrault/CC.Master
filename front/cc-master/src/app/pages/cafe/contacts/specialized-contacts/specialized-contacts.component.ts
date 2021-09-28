import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ISpecializedContact } from './specialized-contact.interface';
import { SpecializedContactsDataService } from './specialized-contacts-data.service';


@Component({
  selector: 'cc-specialized-contacts',
  templateUrl: './specialized-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SpecializedContactsComponent {
  public get contacts$(): Observable<ISpecializedContact[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);
  private paginatedContacts: PaginatedList<ISpecializedContact>;

  constructor(
    private pagingService: PagingService,
    private contactsService: SpecializedContactsDataService,
  ) {
    this.paginatedContacts = this.getPaginatedSpeContacts$();
  }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private getPaginatedSpeContacts$(): PaginatedList<ISpecializedContact> {
    return this.pagingService.paginate<ISpecializedContact>(
      (httpParams) => this.getSpeContacts$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getSpeContacts$(httpParams: HttpParams): Observable<IPaginatedResult<ISpecializedContact>> {
    return this.contactsService.getSpecializedContacts$(httpParams)
      .pipe(map(res => ({ items: res.items, totalCount: res.count })));
  }
}
