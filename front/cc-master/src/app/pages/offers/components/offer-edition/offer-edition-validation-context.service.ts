import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CountResponse } from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class OfferEditionValidationContextService {
  private readonly countsEndpoint = '/api/v3/counts';

  constructor(private httpClient: HttpClient) {
  }

  public getRealCountNumber$(offerId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('code', CountCode.Count)
      .set('offerId', `${ offerId }`);

    return this.httpClient.get<IHttpApiV3CountResponse>(this.countsEndpoint, { params })
      .pipe(map(res => res.data.count));
  }
}
