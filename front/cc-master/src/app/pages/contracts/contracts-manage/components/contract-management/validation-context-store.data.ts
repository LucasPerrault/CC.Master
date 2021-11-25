import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3CountResponse } from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { AccountType } from './tabs/accounting-tab/enums/account-type.enum';
import { AccountingEntryJournalCodes } from './tabs/accounting-tab/enums/accounting-entry-journal-code.enum';

class ValidationContextRoute {
  public static counts = '/api/v3/counts';
  public static attachments = '/api/v3/contractentities';
  public static contractEntries = '/api/v3/contractentries';
}

export interface IContractEntry {
  id: number;
  letter: number;
}

export interface IValidationContext {
  activeEstablishmentNumber: number;
  realCountNumber: number;
  contractEntries: IContractEntry[];
}

@Injectable()
export class ValidationContextDataService {

  constructor(private httpClient: HttpClient) {}

  public getActiveEstablishmentNumber$(contractId: number): Observable<number> {
    const url = ValidationContextRoute.attachments;
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('legalEntity.isActive', String(true));

    return this.httpClient.get<IHttpApiV3CountResponse>(url, { params })
      .pipe(map(response => response.data.count));
  }

  public getRealCountNumber$(contractId: number): Observable<number> {
    const url = ValidationContextRoute.counts;
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('contractId', String(contractId))
      .set('code', CountCode.Count);

    return this.httpClient.get<IHttpApiV3CountResponse>(url, { params })
      .pipe(map(response => response.data.count));
  }

  public getContractEntries$(contractId: number): Observable<IContractEntry[]> {
    const url = ValidationContextRoute.contractEntries;
    const accounts = [AccountType.Client, AccountType.Lucca, AccountType.DeliveryAccount];

    const params = new HttpParams()
      .set('fields', 'id,letter')
      .set('accountNumber', `like,${ accounts.join(',') }`)
      .set('journalCode', `notequal,${ AccountingEntryJournalCodes.Draft }`)
      .set('contractId', String(contractId));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContractEntry>>(url, { params })
      .pipe(map(response => response.data.items));
  }
}
