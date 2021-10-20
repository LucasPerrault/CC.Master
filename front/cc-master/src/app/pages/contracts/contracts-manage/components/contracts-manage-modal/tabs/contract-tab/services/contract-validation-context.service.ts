import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CountResponse } from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { combineLatest, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContractValidationContext } from '../models/contract-validation-context.interface';

@Injectable()
export class ContractValidationContextService {
  private readonly countsEndpoint = '/api/v3/counts';
  private readonly attachmentsEndpoint = '/api/v3/contractentities';
  private readonly contractEntriesEndpoint = '/api/v3/contractentries';

  constructor(private httpClient: HttpClient) {}

  public getValidationContext$(contractId: number): Observable<IContractValidationContext> {
    return combineLatest([
      this.getActiveEstablishmentsNumber$(contractId),
      this.getRealCountNumber$(contractId),
      this.getUnletteredContractEntriesNumber$(contractId),
      this.getLetteredContractEntriesNumber$(contractId),
    ]).pipe(
      map(([activeEstablishmentsNumber, realCountNumber, unletteredContractEntriesNumber, letteredContractEntriesNumber]) => ({
        activeEstablishmentsNumber, realCountNumber, unletteredContractEntriesNumber, letteredContractEntriesNumber,
      })),
    );
  }

  private getActiveEstablishmentsNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('isActive', String(true));

    return this.httpClient.get<IHttpApiV3CountResponse>(this.attachmentsEndpoint, { params })
      .pipe(map(response => response.data.count));
  }

  private getRealCountNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('code', CountCode.Count);

    return this.httpClient.get<IHttpApiV3CountResponse>(this.countsEndpoint, { params })
      .pipe(map(response => response.data.count));
  }

  private getUnletteredContractEntriesNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('letter', String(null));

    return this.httpClient.get<IHttpApiV3CountResponse>(this.contractEntriesEndpoint, { params })
      .pipe(map(response => response.data.count));
  }

  private getLetteredContractEntriesNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('letter', `notequal,null`);

    return this.httpClient.get<IHttpApiV3CountResponse>(this.contractEntriesEndpoint, { params })
      .pipe(map(response => response.data.count));
  }
}
