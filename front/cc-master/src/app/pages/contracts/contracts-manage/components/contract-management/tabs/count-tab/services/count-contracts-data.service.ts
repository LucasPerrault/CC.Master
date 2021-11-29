import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { DownloadService } from '@cc/common/download';
import { ApiV3DateService, IHttpApiV3CollectionResponse, IHttpApiV3Response } from '@cc/common/queries';
import { endOfMonth, startOfMonth } from 'date-fns';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { countContractFields, ICountContract } from '../models/count-contract.interface';
import { detailedCountFields, IDetailedCount } from '../models/detailed-count.interface';

@Injectable()
export class CountContractsDataService {
  private readonly contractsEndpoint = '/api/v3/contracts';
  private readonly countsEndpoint = '/api/v3/counts';
  private readonly newContractsEndpoint = '/api/v3/newcontracts';

  constructor(
    private httpClient: HttpClient,
    private apiV3DateService: ApiV3DateService,
    private downloadService: DownloadService,
  ) {}

  public getCountContract$(id: number): Observable<ICountContract> {
    const urlById = `${ this.newContractsEndpoint }/${ id }`;
    const params = new HttpParams().set('fields', countContractFields);

    return this.httpClient.get<IHttpApiV3Response<ICountContract>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  public getDetailedCounts$(contractId: number): Observable<IDetailedCount[]> {
    const params = new HttpParams()
      .set('contractId', String(contractId))
      .set('fields', detailedCountFields);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IDetailedCount>>(this.countsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }

  public download$(contractId: number, period: IDateRange): Observable<void> {
    const from = this.apiV3DateService.toApiV3DateFormat(startOfMonth(period.startDate));
    const to = this.apiV3DateService.toApiV3DateFormat(endOfMonth(period.endDate));

    const url = `${ this.countsEndpoint }/exportdetails`;
    const body = { contractId, from, to };
    return this.downloadService.download$(url, body);
  }

  public charge$(contractId: number, from: Date, to: Date): Observable<void> {
    const url = `${ this.contractsEndpoint }/${ contractId }/charges`;
    const chargeDto = {
      startPeriod: this.apiV3DateService.toApiV3DateFormat(from),
      endPeriod: this.apiV3DateService.toApiV3DateFormat(to),
    };

    return this.httpClient.post<void>(url, chargeDto);
  }

  public deleteDraftCount$(contractId: number): Observable<void> {
    const url = `${ this.contractsEndpoint }/${ contractId }/deleteDraftCounts`;
    return this.httpClient.post<void>(url, {});
  }

  public deleteRange$(counts: IDetailedCount[]): Observable<void> {
    const request$ = counts.map(count => this.delete$(count));
    return forkJoin(request$).pipe(switchMap( () => of<void>(null)));
  }

  private delete$(count: IDetailedCount): Observable<void> {
    const url = `${ this.countsEndpoint }/${ count.id }`;
    return this.httpClient.delete<void>(url);
  }
}
