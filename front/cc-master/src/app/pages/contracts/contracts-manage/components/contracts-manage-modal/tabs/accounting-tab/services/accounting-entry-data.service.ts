import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { accountingEntryFields, IAccountingEntry } from '../models/accounting-entry.interface';

@Injectable()
export class AccountingEntryDataService {
  private readonly accountingEndpoint = 'api/v3/contractEntries';

  constructor(private httpClient: HttpClient) {}

  public getAccountingEntries$(httpParams: HttpParams): Observable<IHttpApiV3CollectionCount<IAccountingEntry>> {
    const params = httpParams.set('fields', accountingEntryFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IAccountingEntry>>(this.accountingEndpoint, { params })
      .pipe(map(response => response.data));
  }
}
