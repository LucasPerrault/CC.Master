import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { detailedOfferFields, IDetailedOffer } from '../models/detailed-offer.interface';

@Injectable()
export class OffersDataService {
  private readonly offersEndpoint = '/api/v3/offers';

  constructor(private httpClient: HttpClient) {
  }

  public getOffers$(params: HttpParams): Observable<IHttpApiV3CollectionCount<IDetailedOffer>> {
    params = params.set('fields', detailedOfferFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IDetailedOffer>>(this.offersEndpoint, { params })
      .pipe(map(res => res.data));
  }
}
