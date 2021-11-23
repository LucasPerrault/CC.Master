import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { defaultPagingParams } from '@cc/common/paging';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map, mergeMap, reduce, tap } from 'rxjs/operators';

import { IDetailedOfferWithoutUsage } from '../../../models/detailed-offer.interface';
import { IPriceListOfferSelectOption } from './offer-price-list-selection.interface';

@Injectable()
export class OfferPriceListApiSelectService {
  private api = '/api/commercial-offers';
  private fields = `fields.root=count`;

  get url() {
    return `${this.api}?${ this.fields }`;
  }

  constructor(private http: HttpClient) { }

  public searchPaged(clue: string, page: number, filters: string[] = []): Observable<IPriceListOfferSelectOption[]> {
    if (!clue) {
      return this.getPaged(page, filters);
    }
    const paging = `page=${page + 1}&limit=${defaultPagingParams.limit}`;
    const url = [this.url, this.clueFilter(clue), paging, ...filters].join('&');
    return this.get(url);
  }

  public getPaged(page: number, filters: string[] = []): Observable<IPriceListOfferSelectOption[]> {
    const paging = `page=${page + 1}&limit=${defaultPagingParams.limit}`;
    const url = [this.url, paging, ...filters].join('&');
    return this.get(url);
  }

  private clueFilter(clue) {
    const urlSafeClue = encodeURIComponent(clue);
    return `search=${ urlSafeClue }`;
  }

  private get(url): Observable<IPriceListOfferSelectOption[]> {
    return this.http.get<IHttpApiV4CollectionCountResponse<IDetailedOfferWithoutUsage>>(url).pipe(
      map(response => response.items),
      tap(items => console.log(items)),
      map(offers => offers.map(offer => this.toSelection(offer))),
      mergeMap(x => x),
      reduce((acc, x) => [...acc, ...x]),
    );
  }

  private toSelection(offer: IDetailedOfferWithoutUsage): IPriceListOfferSelectOption[] {
    return offer.priceLists.map(priceList => ({
      id: offer.id,
      name: offer.name,
      priceList,
    }));
  }
}

