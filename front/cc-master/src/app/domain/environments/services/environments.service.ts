import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { IEnvironment } from '@cc/domain/environments';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable()
export class EnvironmentsService {

  private readonly environmentsEndPoint = '/api/v3/environments';

  constructor(private httpClient: HttpClient) { }

  public getEnvironmentsById$(ids: number[]): Observable<IEnvironment[]> {
    const fields = 'id,subdomain';

    const params = new HttpParams()
      .set('fields', fields)
      .set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IEnvironment>>(this.environmentsEndPoint, { params })
      .pipe(
        map(response => response.data),
        map(data => data.items),
      );
  }
}
