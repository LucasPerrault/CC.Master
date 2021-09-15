import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { countFields, ICount } from '@cc/domain/billing/counts/count.interface';
import { CountCode } from '@cc/domain/billing/counts/count-code.enum';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CountsService {
  private readonly countsEndpoint = '/api/v3/counts';

  constructor(private httpClient: HttpClient) {}

  public getRealCounts$(contractId: number): Observable<ICount[]> {
    const params = new HttpParams()
      .set('fields', countFields)
      .set('contractId', String(contractId))
      .set('code', CountCode.Count);

    return this.httpClient.get<IHttpApiV3CollectionResponse<ICount>>(this.countsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
