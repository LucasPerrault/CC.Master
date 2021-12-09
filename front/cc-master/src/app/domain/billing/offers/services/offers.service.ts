import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IOffer } from '../models/offer.interface';

@Injectable()
export class OffersService {

  private readonly offersEndPoint = '/api/commercial-offers';

  constructor(private httpClient: HttpClient) { }

  public getOffersById$(ids: number[]): Observable<IOffer[]> {
    const params = new HttpParams().set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV4CollectionResponse<IOffer>>(this.offersEndPoint, { params })
      .pipe(map(response => response.items));
  }
}
