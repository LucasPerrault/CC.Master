import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { IGenericContact } from './generic-contact.interface';

@Injectable()
export class GenericContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getContacts$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IGenericContact>> {
    params = params.set('field.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/contacts/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IGenericContact>>(url, { });
  }
}
