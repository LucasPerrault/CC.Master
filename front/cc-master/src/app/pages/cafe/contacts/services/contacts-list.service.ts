import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContactListEntry } from '../components/contact-list/contact-list.interface';
import { ContactCategory } from '../enums/cafe-contacts-category.enum';
import { IAppContact } from '../models/app-contact.interface';
import { IClientContact } from '../models/client-contact.interface';
import { ICommonContact } from '../models/common-contact.interface';
import { IContact } from '../models/contact.interface';
import { ISpecializedContact } from '../models/specialized-contact.interface';
import { AppContactsDataService } from './app-contacts-data.service';
import { ClientContactsDataService } from './client-contacts-data.service';
import { CommonContactsDataService } from './common-contacts-data.service';
import { ContactRolesService } from './contact-roles.service';
import { SpecializedContactsDataService } from './specialized-contacts-data.service';

@Injectable()
export class ContactsListService {
  constructor(
    private clientContactsDataService: ClientContactsDataService,
    private appContactsDataService: AppContactsDataService,
    private speContactsDataService: SpecializedContactsDataService,
    private commonContactsDataService: CommonContactsDataService,
    private rolesService: ContactRolesService,
    private pagingService: PagingService,
  ) {
  }

  public getPaginatedContacts$(category: ContactCategory): PaginatedList<IContactListEntry> {
    return this.pagingService.paginate<IContactListEntry>(
      (httpParams) => this.getContacts$(httpParams, category),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getContacts$(httpParams: HttpParams, category: ContactCategory): Observable<IPaginatedResult<IContactListEntry>> {
    switch (category) {
      case ContactCategory.All:
        return this.getAllContacts$(httpParams);
      case ContactCategory.Application:
        return this.getAppContacts$(httpParams);
      case ContactCategory.Client:
        return this.getClientContacts$(httpParams);
      case ContactCategory.Specialized:
        return this.getSpeContacts$(httpParams);
    }
  }

  private getAllContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IContactListEntry>> {
    return this.commonContactsDataService.getContacts$(httpParams)
      .pipe(map(res => ({ items: this.getAllContactsAsListEntry(res.items), totalCount: res.count })));
  }

  private getAllContactsAsListEntry(contacts: ICommonContact[]): IContactListEntry[] {
    return contacts.map(contact => {
      const role = this.rolesService.getCommonContactRole(contact?.roleCode);
      return { ...this.toListEntryWithoutRole(contact), role };
    });
  }

  private getAppContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IContactListEntry>> {
    return this.appContactsDataService.getAppContacts$(httpParams)
      .pipe(map(res => ({ items: this.getAppContactsAsListEntry(res.items), totalCount: res.count })));
  }

  private getAppContactsAsListEntry(contacts: IAppContact[]): IContactListEntry[] {
    return contacts.map(contact => {
      const role = this.rolesService.getAppContactRole(contact?.appInstance);
      return { ...this.toListEntryWithoutRole(contact), role };
    });
  }

  private getClientContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IContactListEntry>> {
    return this.clientContactsDataService.getClientContacts$(httpParams).pipe(
      map(res => ({ items: this.getClientContactsAsListEntry(res.items), totalCount: res.count })));
  }

  private getClientContactsAsListEntry(contacts: IClientContact[]): IContactListEntry[] {
    return contacts.map(contact => {
      const role = this.rolesService.getClientContactRole(contact?.roleCode);
      return { ...this.toListEntryWithoutRole(contact), role };
    });
  }

  private getSpeContacts$(httpParams: HttpParams): Observable<IPaginatedResult<IContactListEntry>> {
    return this.speContactsDataService.getSpecializedContacts$(httpParams).pipe(
      map(res => ({ items: this.getSpeContactsAsListEntry(res.items), totalCount: res.count })),
    );
  }

  private getSpeContactsAsListEntry(contacts: ISpecializedContact[]): IContactListEntry[] {
    return contacts.map(contact => {
      const role = this.rolesService.getSpeContactRole(contact.roleCode);
      return { ...this.toListEntryWithoutRole(contact), role };
    });
  }

  private toListEntryWithoutRole(contact: IContact): Omit<IContactListEntry, 'role'> {
    return {
      id: contact.id,
      isConfirmed: contact.isConfirmed,
      createdAt: contact.createdAt,
      expiredAt: contact.expiresAt,
      lastname: contact.user.lastName,
      firstname: contact.user.firstName,
      mail: contact.user.mail,
      environmentId: contact.environmentId,
      environment: contact.environment,
    };
  }
}
