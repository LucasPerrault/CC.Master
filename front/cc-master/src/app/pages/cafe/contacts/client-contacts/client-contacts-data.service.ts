import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/cafe-filters/advanced-filter-form';
import { IClientContact } from './client-contact.interface';

@Injectable()
export class ClientContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getClientContacts$(
    params: HttpParams,
    advancedFilter: AdvancedFilter,
  ): Observable<IHttpApiV4CollectionCountResponse<IClientContact>> {
    params = params.set('field.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/client-contacts/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IClientContact>>(url, advancedFilter);
  }
}
