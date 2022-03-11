import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ApiSortHelper,
  ApiV3DateService,
  IHttpApiV3CollectionCountResponse, IHttpApiV3CollectionResponse, IHttpApiV3CountResponse, IHttpApiV4CollectionCountResponse,
} from '@cc/common/queries';
import { CountCode } from '@cc/domain/billing/counts';
import { addDays } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { countWithContractFields, ICountWithContract } from '../models/count-with-contract.interface';
import { IMissingCount } from '../models/missing-count.interface';

@Injectable()
export class CountsDataService {
  constructor(private httpClient: HttpClient, private apiDateService: ApiV3DateService) {}

  public cleanForecast$(): Observable<void> {
    const url = '/api/v3/counts/deleteDraftInPast';
    return this.httpClient.post<void>(url, {});
  }

  public getCounts$(countPeriod: Date): Observable<ICountWithContract[]> {
    const url = '/api/v3/counts';
    const params = new HttpParams()
      .set('fields', countWithContractFields)
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set(ApiSortHelper.v3SortKey, `contract.id,${ ApiSortHelper.v3AscKey }`);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<ICountWithContract>>(url, { params })
      .pipe(map(res => res.data.items));
  }

  public getRealCountsNumber$(countPeriod: Date): Observable<number> {
    const url = '/api/v3/counts';
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set('code', CountCode.Count);

    return this.httpClient.get<IHttpApiV3CountResponse>(url, { params })
      .pipe(map(res => res.data.count));
  }

  public getDraftCounts$(countPeriod: Date): Observable<ICountWithContract[]> {
    const url = '/api/v3/counts';
    const params = new HttpParams()
      .set('fields', countWithContractFields)
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set('code', CountCode.Draft)
      .set(ApiSortHelper.v3SortKey, `contract.id,${ApiSortHelper.v3AscKey}`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<ICountWithContract>>(url, { params })
      .pipe(map(res => res.data.items));
  }

  public getRealCountsWithoutAccountingEntries(countPeriod: Date): Observable<ICountWithContract[]> {
    const url = '/api/v3/counts';
    const params = new HttpParams()
      .set('fields', countWithContractFields)
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set('code', CountCode.Count)
      .set('entryNumber', 'null');

    return this.httpClient.get<IHttpApiV3CollectionResponse<ICountWithContract>>(url, { params })
      .pipe(map(res => res.data.items));
  }

  public getMissingCounts$(countPeriod: Date): Observable<IMissingCount[]> {
    const url = '/api/counts/missing';
    const params = new HttpParams().set('period', this.apiDateService.toApiV3DateFormat(countPeriod));

    return this.httpClient.get<IHttpApiV4CollectionCountResponse<IMissingCount>>(url, { params })
      .pipe(map(res => res.items));
  }

  public getActiveContractsNumber$(countPeriod: Date): Observable<number> {
    const url = '/api/v3/contracts';
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('isConfirmed', 'true')
      .set('archivedAt', 'null')
      .set('startOn', `until,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))}`)
      .set('closeOn', `since,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))},null`);

    return this.httpClient.get<IHttpApiV3CountResponse>(url, { params })
      .pipe(map(res => res.data.count));
  }

  private toMiddleOfMonth(countPeriod: Date): Date {
    return addDays(countPeriod, 14);
  }
}

