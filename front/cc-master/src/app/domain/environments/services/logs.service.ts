import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse } from '@cc/common/queries';
import { IEnvironmentLog } from '@cc/domain/environments';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable()
export class LogsService {

  constructor(private httpClient: HttpClient) { }

  public getLogs$(httpParams: HttpParams): Observable<IHttpApiV3CollectionCount<IEnvironmentLog>> {
    const fields = 'collection.count,id,name,user,isAnonymizedData,activity,createdOn,environment[subDomain,domainName],' +
      'messages[id,message,type]';
    const environmentLogUrl = `/api/v3/environmentLogs`;

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IEnvironmentLog>>(environmentLogUrl, {
      params: httpParams.set('fields', fields),
    }).pipe(map(response => response.data));
  }
}
