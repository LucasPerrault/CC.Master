import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { IEnvironmentGroup } from '@cc/domain/environments/models/environment-group.interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class EnvironmentGroupsService {

  private readonly environmentsEndPoint = '/api/v3/environmentgroups';

  constructor(private httpClient: HttpClient) { }

  public getEnvironmentGroupsByIds$(ids: number[]): Observable<IEnvironmentGroup[]> {
    const params = new HttpParams()
      .set('fields', 'id,name')
      .set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IEnvironmentGroup>>(this.environmentsEndPoint, { params })
      .pipe(map(response => response.data.items));
  }
}
