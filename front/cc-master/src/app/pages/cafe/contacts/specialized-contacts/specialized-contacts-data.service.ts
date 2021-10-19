import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/cafe-filters/advanced-filter-form';
import { ISpecializedContact } from './specialized-contact.interface';

@Injectable()
export class SpecializedContactsDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getSpecializedContacts$(
    params: HttpParams,
    advancedFilter: AdvancedFilter,
  ): Observable<IHttpApiV4CollectionCountResponse<ISpecializedContact>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/specialized-contacts/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<ISpecializedContact>>(url, advancedFilter);
  }
}
