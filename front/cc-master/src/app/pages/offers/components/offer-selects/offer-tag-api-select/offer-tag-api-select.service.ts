import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IOffer } from '@cc/domain/billing/offers';
import { ILuApiCollectionResponse } from '@lucca-front/ng/api';
import { from, Observable } from 'rxjs';
import { concatMap, distinct, map, reduce } from 'rxjs/operators';

interface ITagOffer extends IOffer {
  tag: string;
}

@Injectable()
export class OfferTagApiSelectService {
  private api = '/api/v3/offers';
  private fields = 'fields=id,name,tag';

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
    return `tag=like,${urlSafeClue}`;
  }

  private get(url): Observable<string[]> {
    return this.http.get<ILuApiCollectionResponse<ITagOffer>>(url).pipe(
      map(response => response.data.items),
      concatMap((x) => from(x)),
      map((o: ITagOffer) => this.capitalize(o.tag)),
      distinct(),
      reduce((tags, tag) => [...tags, tag], []),
      map((tags: string[]) => tags.sort()),
    );
  }

  private capitalize(tag: string): string {
    return tag[0].toUpperCase() + tag.slice(1);
  }
}

