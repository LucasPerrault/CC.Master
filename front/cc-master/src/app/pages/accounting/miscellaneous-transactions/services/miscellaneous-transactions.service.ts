import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiV3SortKey, apiV3SortOrderAscendingKey, apiV3SortOrderDescendingKey, IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IMiscellaneousTransaction, miscTransactionFields } from '../models/miscellaneous-transaction.interface';

class MiscTransactionEndPoint {
  public static base = '/api/v3/miscellaneousTransactions';
}

@Injectable()
export class MiscellaneousTransactionsService {
  constructor(private httpClient: HttpClient) {}

  public getMiscellaneousTransactions$(): Observable<IMiscellaneousTransaction[]> {
    const params = new HttpParams()
      .set('fields', miscTransactionFields)
      .set(apiV3SortKey, `periodOn,${ apiV3SortOrderDescendingKey },contract.Id,${ apiV3SortOrderAscendingKey }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IMiscellaneousTransaction>>(MiscTransactionEndPoint.base, { params })
      .pipe(map(res => res.data.items));
  }
}
