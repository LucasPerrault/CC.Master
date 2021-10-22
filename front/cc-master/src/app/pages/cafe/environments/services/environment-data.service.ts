import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/cafe-filters/advanced-filter-form';
import { IEnvironment } from '../models/environment.interface';

@Injectable()
export class EnvironmentDataService {
  constructor(private httpClient: HttpClient) {
  }

  public getEnvironments$(params: HttpParams, advancedFilter: AdvancedFilter): Observable<IHttpApiV4CollectionCountResponse<IEnvironment>> {
    params = params.set('field.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/environments/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IEnvironment>>(url, advancedFilter);
  }
}
