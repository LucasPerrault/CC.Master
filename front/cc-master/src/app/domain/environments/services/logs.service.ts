import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IFilterParams } from '@cc/common/filter';
import { IPagingParams } from '@cc/common/paging';
import { IHttpApiV3CollectionCountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { ISortParams } from '@cc/common/sort';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IEnvironmentLog } from '../../../pages/logs/models';

@Injectable()
export class LogsService {

  constructor(private httpClient: HttpClient) { }

  public getLogs$(
    pagingParams: IPagingParams,
    sortParams: ISortParams,
    queryParams?: IFilterParams,
  ): Observable<IHttpApiV3CollectionCountResponse<IEnvironmentLog>> {
    const fields = 'collection.count,id,name,user,isAnonymizedData,activity,createdOn,environment[subDomain,domainName],' +
      'messages[id,message,type]';
    const environmentLogUrl = `/api/v3/environmentLogs`;

    return this.httpClient.get<IHttpApiV3Response<IHttpApiV3CollectionCountResponse<IEnvironmentLog>>>(environmentLogUrl, {
      params: {
        ...queryParams,
        fields,
        paging: `${pagingParams.skip},${pagingParams.limit}`,
        orderBy: `${sortParams.field},${sortParams.order}`,
      },
    }).pipe(map(response => response.data));
  }
}
