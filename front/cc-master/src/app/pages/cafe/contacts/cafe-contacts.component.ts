import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { PaginatedList, PaginatedListState } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContactListEntry } from './components/contact-list/contact-list.interface';
import { ContactCategory } from './enums/cafe-contacts-category.enum';
import { ContactsListService } from './services/contacts-list.service';

@Component({
  selector: 'cc-cafe-contacts',
  templateUrl: './cafe-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeContactsComponent {
  @Input() public set category(category: ContactCategory) {
    this.reset(category);
  }

  public get contacts$(): Observable<IContactListEntry[]> {
    return this.paginatedContacts.items$;
  }

  public get contactsCount$(): Observable<number> {
    return this.paginatedContacts.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContacts.state$.pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);

  private paginatedContacts: PaginatedList<IContactListEntry>;

  constructor(private contactsService: ContactsListService) { }

  public nextPage(): void {
    this.paginatedContacts.nextPage();
  }

  private reset(category: ContactCategory): void {
    this.paginatedContacts = this.contactsService.getPaginatedContacts$(category);
  }
}
