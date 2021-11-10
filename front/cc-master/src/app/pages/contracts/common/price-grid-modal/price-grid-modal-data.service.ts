import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Response } from '@cc/common/queries';
import { IPriceListOffer, offerPriceListFields } from '@cc/domain/billing/offers/models/offer-price-list.interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class PriceGridModalDataService {

  private readonly offersEndPoint = '/api/v3/offers';

  constructor(private httpClient: HttpClient) { }

  public getOfferPriceList$(offerId: number): Observable<IPriceListOffer> {
    const url = `${this.offersEndPoint}/${offerId}`;
    const params = new HttpParams().set('fields', offerPriceListFields);

    return this.httpClient.get<IHttpApiV3Response<IPriceListOffer>>(url, { params })
      .pipe(map(response => response.data));
  }
}
