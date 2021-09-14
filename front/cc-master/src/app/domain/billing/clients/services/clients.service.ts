import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { clientFields, IClient } from '../models/client.interface';

@Injectable()
export class ClientsService {

  private readonly clientsEndPoint = '/api/v3/clients';

  constructor(private httpClient: HttpClient) { }

  public getClientsById$(ids: number[]): Observable<IClient[]> {
    const params = new HttpParams().set('fields', clientFields).set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IClient>>(this.clientsEndPoint, { params }).pipe(
      map(response => response.data),
      map(data => data.items),
    );
  }
}
