import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { establishmentFields, IEstablishment } from '../models/establishment.interface';

@Injectable()
export class EstablishmentsService {

  private readonly establishmentsEndpoint = '/api/v3/legalentities';

  constructor(private httpClient: HttpClient) { }

  public getEstablishmentsById$(ids: number[]): Observable<IEstablishment[]> {
    const params = new HttpParams().set('fields', establishmentFields).set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IEstablishment>>(this.establishmentsEndpoint, { params }).pipe(
      map(response => response.data),
      map(data => data.items),
    );
  }
}
