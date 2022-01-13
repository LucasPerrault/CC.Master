import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DownloadService } from '@cc/common/download';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { detailedCountFields, IDetailedCount } from '../models/detailed-count.interface';

@Injectable()
export class CountsDataService {
  constructor(private httpClient: HttpClient, private downloadService: DownloadService) {}

  public getDetailedCounts$(httpParams: HttpParams): Observable<IHttpApiV3CollectionCount<IDetailedCount>> {
    const url = '/api/v3/counts';
    const params = httpParams.set('fields', detailedCountFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IDetailedCount>>(url, { params })
      .pipe(map(response => response.data));
  }

  public export$(params: HttpParams): Observable<void> {
    const url = '/api/v3/counts/export';
    return this.downloadService.download$([url, params.toString()].join('?'));
  }
}
