import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ApiSortHelper,
  ApiV3DateService, IHttpApiV3CollectionCount,
  IHttpApiV3CollectionCountResponse, IHttpApiV3CountResponse,
} from '@cc/common/queries';
import { contractFields, IContract } from '@cc/domain/billing/contracts';
import { CountCode } from '@cc/domain/billing/counts';
import { addDays } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { countWithContractFields, ICountWithContract } from '../models/count-with-contract.interface';

@Injectable()
export class CountsDataService {
  constructor(private httpClient: HttpClient, private apiDateService: ApiV3DateService) {}

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

  public getDraftCounts$(httpParams: HttpParams, countPeriod: Date): Observable<IHttpApiV3CollectionCount<ICountWithContract>> {
    const url = '/api/v3/counts';
    const params = httpParams
      .set('fields', `collection.count,${ countWithContractFields }`)
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set('code', CountCode.Draft)
      .set(ApiSortHelper.v3SortKey, `contract.id,${ApiSortHelper.v3AscKey}`);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<ICountWithContract>>(url, { params })
      .pipe(map(res => res.data));
  }

  public getRealCountsWithoutAccountingEntries(
    httpParams: HttpParams,
    countPeriod: Date,
  ): Observable<IHttpApiV3CollectionCount<ICountWithContract>> {
    const url = '/api/v3/counts';
    const params = httpParams
      .set('fields', `collection.count,${ countWithContractFields }`)
      .set('countPeriod', this.apiDateService.toApiV3DateFormat(countPeriod))
      .set('code', CountCode.Count)
      .set('entryNumber', 'null');

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<ICountWithContract>>(url, { params })
      .pipe(map(res => res.data));
  }

  public getContracts$(countPeriod: Date): Observable<IContract[]> {
    const url = '/api/v3/newcontracts';
    const params = new HttpParams()
      .set('fields', contractFields)
      .set('environmentId', 'notequal,null')
      .set('archivedAt', 'null')
      .set('startOn', `until,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))}`)
      .set('closeOn', `since,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))},null`)
      .set(ApiSortHelper.v3SortKey, `id,${ ApiSortHelper.v3AscKey }`);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IContract>>(url, { params })
      .pipe(map(res => res.data.items));
  }

  public getActiveContractsNumber$(countPeriod: Date): Observable<number> {
    const url = '/api/v3/contracts';
    const params = new HttpParams()
      .set('fields', 'collection.count')
      .set('isConfirmed', 'true')
      .set('archivedAt', 'null')
      .set('startOn', `until,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))}`)
      .set('closeOn', `since,${ this.apiDateService.toApiV3DateFormat(this.toMiddleOfMonth(countPeriod))},null`);

    return this.httpClient.get<number>(url, { params });
  }

  private toMiddleOfMonth(countPeriod: Date): Date {
    return addDays(countPeriod, 14);
  }

}

