import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Collection, IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { contractLogFields, IContractLog } from '../models/contract-log.interface';

@Injectable()
export class ContractLogsService {
  private readonly contractsEndpoint = `/api/v3/contracts`;

  constructor(private httpClient: HttpClient) {}

  public getContractLogs$(contractId: number): Observable<IContractLog[]> {
    const params = new HttpParams().set('fields', contractLogFields);
    const url = `${ this.contractsEndpoint }/${ contractId }/logs`;

    return this.httpClient.get<IHttpApiV3Response<IHttpApiV3Collection<IContractLog>>>(url, { params })
      .pipe(map(response => response.data.items));
  }
}
