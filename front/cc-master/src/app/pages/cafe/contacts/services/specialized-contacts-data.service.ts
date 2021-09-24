import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

import { ISpecializedContact } from '../models/specialized-contact.interface';

@Injectable()
export class SpecializedContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getSpecializedContacts$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<ISpecializedContact>> {
    params = params.set('field.root', 'count');

    const url = '/api/cafe/specialized-contacts';

    return this.httpClient.get<IHttpApiV4CollectionCountResponse<ISpecializedContact>>(url, { params }).pipe(
      catchError(() => of({ items: [], count: 0 })),
      delay(2000),
    );
  }
}
