import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOffer } from '@cc/domain/billing/offers';
import { ILuApiCollectionResponse } from '@lucca-front/ng/api';
import { from, Observable } from 'rxjs';
import { concatMap, distinct, map, reduce } from 'rxjs/operators';

import { IOfferCurrency, offerCurrencyFields } from '../../../models/offer-currency.interface';

interface ICurrencyOffer extends IOffer {
  currency: IOfferCurrency;
}

@Injectable()
export class OfferCurrencyApiSelectService {
  private api = '/api/v3/offers';
  private fields = `fields=id,name,currency[${ offerCurrencyFields }]`;

  get url() {
    return `${this.api}?${ this.fields }`;
  }

  constructor(private http: HttpClient) { }

  getAll(filters: string[] = []): Observable<IOfferCurrency[]> {
    return this.get([this.url, ...filters].join('&'));
  }

  searchAll(clue: string, filters: string[] = []): Observable<IOfferCurrency[]> {
    if (!clue) {
      return this.getAll(filters);
    }
    const url = [this.url, this.clueFilter(clue), ...filters].join('&');
    return this.get(url);
  }

  private clueFilter(clue) {
    const urlSafeClue = encodeURIComponent(clue);
    return `currency.name=like,${urlSafeClue}`;
  }

  private get(url): Observable<IOfferCurrency[]> {
    return this.http.get<ILuApiCollectionResponse<ICurrencyOffer>>(url).pipe(
      map(response => response.data.items),
      concatMap((x) => from(x)),
      map((o: ICurrencyOffer) => o.currency),
      distinct((currency: IOfferCurrency) => currency.code),
      reduce((currencies, currency) => [...currencies, currency], []),
      map((currencies: IOfferCurrency[]) => currencies.sort((a, b) => a.name.localeCompare(b.name))),
    );
  }
}

