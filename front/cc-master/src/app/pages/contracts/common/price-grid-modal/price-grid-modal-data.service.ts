import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IPriceListOffer } from './price-grid-offer.interface';

@Injectable()
export class PriceGridModalDataService {

  constructor(private httpClient: HttpClient) { }

  public getOfferPriceList$(offerId: number): Observable<IPriceListOffer> {
    const url = `/api/commercial-offers/${offerId}`;
    return this.httpClient.get<IPriceListOffer>(url);
  }
}
