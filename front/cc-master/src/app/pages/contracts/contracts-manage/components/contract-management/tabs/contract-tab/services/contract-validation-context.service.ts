import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3CountResponse } from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { combineLatest, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { AccountType } from '../../accounting-tab/enums/account-type.enum';
import { AccountingEntryJournalCodes } from '../../accounting-tab/enums/accounting-entry-journal-code.enum';
import { IContractEntry, IContractValidationContext } from '../models/contract-validation-context.interface';

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
      this.getContractEntries$(contractId),
    ]).pipe(
      map(([activeEstablishmentsNumber, realCountNumber, contractEntries]) => ({
        activeEstablishmentsNumber, realCountNumber, contractEntries,
      })),
    );
  }

  private getActiveEstablishmentsNumber$(contractId: number): Observable<number> {
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('legalEntity.isActive', String(true));

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

  private getContractEntries$(contractId: number): Observable<IContractEntry[]> {
    const accounts = [AccountType.Client, AccountType.Lucca, AccountType.DeliveryAccount];

    const params = new HttpParams()
      .set('fields', 'id,letter')
      .set('accountNumber', `like,${ accounts.join(',') }`)
      .set('journalCode', `notequal,${ AccountingEntryJournalCodes.Draft }`)
      .set('contractId', String(contractId));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContractEntry>>(this.contractEntriesEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
