import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { IPriceList } from '@cc/domain/billing/offers';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { detailedOfferFields, IDetailedOffer } from '../models/detailed-offer.interface';
import { IPriceListsOffer, priceListsOfferFields } from '../models/price-lists-offer.interface';

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

  public delete$(offerId: number): Observable<void> {
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.delete<void>(url);
  }

  public getPriceLists$(offerId: number): Observable<IPriceList[]> {
    const params = new HttpParams().set('fields', priceListsOfferFields);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.get<IHttpApiV3Response<IPriceListsOffer>>(url, { params })
      .pipe(map(res => res.data.priceLists));
  }
}
