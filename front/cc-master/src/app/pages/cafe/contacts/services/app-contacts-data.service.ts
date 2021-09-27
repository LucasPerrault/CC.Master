import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

import { IAppContact } from '../models/app-contact.interface';

@Injectable()
export class AppContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getAppContacts$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IAppContact>> {
    params = params.set('field.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/app-contacts/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IAppContact>>(url, {}).pipe(
      catchError(() => of({ items: [], count: 0 })),
      delay(2000),
    );
  }
}
