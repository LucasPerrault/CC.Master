import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';

import { IEstablishment } from '../../common/models/establishment.interface';

@Injectable()
export class EstablishmentsDataService {
  constructor(private httpClient: HttpClient) {}

  public getEstablishments$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IEstablishment>> {
    params = params.set('fields.root', 'count');

    const query = params.toString();
    const route = '/api/cafe/establishments/search';
    const url = [route, query].join('?');

    return this.httpClient.post<IHttpApiV4CollectionCountResponse<IEstablishment>>(url, {});
  }
}
