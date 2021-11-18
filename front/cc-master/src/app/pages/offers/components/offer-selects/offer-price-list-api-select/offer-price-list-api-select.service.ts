import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { defaultPagingParams } from '@cc/common/paging';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { IPriceList, priceListFields } from '@cc/domain/billing/offers';
import { Observable } from 'rxjs';
import { map, mergeMap, reduce } from 'rxjs/operators';

import { IPriceListOfferSelectOption } from './offer-price-list-selection.interface';

interface IPriceListOffer {
  id: number;
  name: string;
  priceLists: IPriceList[];
}

@Injectable()
export class OfferPriceListApiSelectService {
  private api = '/api/v3/offers';
  private fields = `fields=id,name,priceLists[${ priceListFields }]`;
  private orderBy = 'orderBy=name,asc';

  get url() {
    return `${this.api}?${ this.fields }&${ this.orderBy }`;
  }

  constructor(private http: HttpClient) { }

  public searchPaged(clue: string, page: number, filters: string[] = []): Observable<IPriceListOfferSelectOption[]> {
    if (!clue) {
      return this.getPaged(page, filters);
    }
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, this.clueFilter(clue), paging, ...filters].join('&');
    return this.get(url);
  }

  public getPaged(page: number, filters: string[] = []): Observable<IPriceListOfferSelectOption[]> {
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, paging, ...filters].join('&');
    return this.get(url);
  }

  private clueFilter(clue) {
    const urlSafeClue = encodeURIComponent(clue);
    return `name=like,${ urlSafeClue }`;
  }

  private get(url): Observable<IPriceListOfferSelectOption[]> {
    return this.http.get<IHttpApiV3CollectionResponse<IPriceListOffer>>(url).pipe(
      map(response => response.data.items),
      map(offers => offers.map(offer => this.toSelection(offer))),
      mergeMap(x => x),
      reduce((acc, x) => [...acc, ...x]),
    );
  }

  private toSelection(offer: IPriceListOffer): IPriceListOfferSelectOption[] {
    return offer.priceLists.map(priceList => ({
      id: offer.id,
      name: offer.name,
      priceList,
    }));
  }
}

