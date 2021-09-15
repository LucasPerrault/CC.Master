import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IOffer, offerFields } from '../models/offer.interface';

@Injectable()
export class OffersService {

  private readonly offersEndPoint = '/api/v3/offers';

  constructor(private httpClient: HttpClient) { }

  public getOffersById$(ids: number[]): Observable<IOffer[]> {
    const params = new HttpParams().set('fields', offerFields).set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IOffer>>(this.offersEndPoint, { params }).pipe(
      map(response => response.data),
      map(data => data.items),
    );
  }
}
