import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse } from '@cc/common/queries';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

import { contractListEntryFields, IContractListEntry } from '../models/contract-list-entry.interface';

@Injectable()
export class ContractsListService {
  private readonly contractsEndpoint = '/api/v3/newcontracts';
  private readonly exportContractsEndpoint = `${ this.contractsEndpoint }/export`;

  private onRefresh: Subject<void> = new Subject<void>();

  public get onRefresh$(): Observable<void> {
    return this.onRefresh.asObservable();
  }

  constructor(private httpClient: HttpClient) {}

  public refresh(): void {
    this.onRefresh.next();
  }

  public getContractsList$(httpParams: HttpParams): Observable<IHttpApiV3CollectionCount<IContractListEntry>> {
    const params = httpParams.set('fields', contractListEntryFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IContractListEntry>>(this.contractsEndpoint, { params })
      .pipe(map(response => response.data));
  }

  public getExportContractsUrl(httpParams: HttpParams): string {
    return `${ this.exportContractsEndpoint }?${ httpParams.toString() }`;
  }
}
