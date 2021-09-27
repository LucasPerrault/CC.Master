import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

import { ICommonContact } from '../models/common-contact.interface';

@Injectable()
export class CommonContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getContacts$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<ICommonContact>> {
    params = params.set('field.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/contacts/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<ICommonContact>>(url, { }).pipe(
      catchError(() => of({ items: [], count: 0 })),
      delay(2000),
    );
  }
}
