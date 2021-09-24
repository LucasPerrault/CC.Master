import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

import { IClientContact } from '../models/client-contact.interface';

@Injectable()
export class ClientContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getClientContacts$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IClientContact>> {
    params = params.set('field.root', 'count');

    const url = '/api/cafe/client-contacts';

    return this.httpClient.get<IHttpApiV4CollectionCountResponse<IClientContact>>(url, { params }).pipe(
      catchError(() => of({ items: [], count: 0 })),
      delay(2000),
    );
  }
}
