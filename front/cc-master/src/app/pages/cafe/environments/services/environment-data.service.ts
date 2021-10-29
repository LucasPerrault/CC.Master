import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/cafe-filters/advanced-filter-form';
import { IEnvironment } from '../models/environment.interface';

@Injectable()
export class EnvironmentDataService {
  constructor(private httpClient: HttpClient, private downloadService: DownloadService) {
  }

  public getEnvironments$(params: HttpParams, advancedFilter: AdvancedFilter): Observable<IHttpApiV4CollectionCountResponse<IEnvironment>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/environments/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IEnvironment>>(url, advancedFilter);
  }

  public exportEnvironments$(advancedFilter: AdvancedFilter): Observable<void> {
    const route = '/api/cafe/environments/export';
    return this.downloadService.download$(route, advancedFilter);
  }
}
