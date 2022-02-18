import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { AdvancedFilter } from '../../common/components/advanced-filter-form';
import { toSearchDto } from '../../common/models';
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
    const body = toSearchDto(advancedFilter);

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IEnvironment>>(url, body);
  }

  public exportEnvironments$(advancedFilter: AdvancedFilter): Observable<void> {
    const route = '/api/cafe/environments/export';
    return this.downloadService.download$(route, advancedFilter);
  }
}
