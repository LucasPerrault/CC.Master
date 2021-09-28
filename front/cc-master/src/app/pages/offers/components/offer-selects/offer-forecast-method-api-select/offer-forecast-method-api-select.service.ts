import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOffer } from '@cc/domain/billing/offers';
import { ILuApiCollectionResponse } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';
import { fromArray } from 'rxjs/internal/observable/fromArray';
import { concatMap, distinct, map, reduce } from 'rxjs/operators';

interface IForecastMethodOffer extends IOffer {
  forecastMethod: string;
}

@Injectable()
export class OfferForecastMethodApiSelectService {
  private api = '/api/v3/offers';
  private fields = 'fields=id,name,forecastMethod';

  get url() {
    return `${this.api}?${ this.fields }`;
  }

  constructor(private http: HttpClient) { }

  getAll(filters: string[] = []): Observable<string[]> {
    return this.get([this.url, ...filters].join('&'));
  }

  searchAll(clue: string, filters: string[] = []): Observable<string[]> {
    if (!clue) {
      return this.getAll(filters);
    }
    const url = [this.url, this.clueFilter(clue), ...filters].join('&');
    return this.get(url);
  }

  private clueFilter(clue) {
    const urlSafeClue = encodeURIComponent(clue);
    return `forecastMethod=like,${urlSafeClue}`;
  }

  private get(url): Observable<string[]> {
    return this.http.get<ILuApiCollectionResponse<IForecastMethodOffer>>(url).pipe(
      map(response => response.data.items),
      concatMap((x) => fromArray(x)),
      map((o: IForecastMethodOffer) => this.capitalize(o.forecastMethod)),
      distinct(),
      reduce((forecastMethods, forecastMethod) => [...forecastMethods, forecastMethod], []),
      map((forecastMethods: string[]) => forecastMethods.sort()),
    );
  }

  private capitalize(forecastMethod: string): string {
    return forecastMethod[0].toUpperCase() + forecastMethod.slice(1);
  }
}

