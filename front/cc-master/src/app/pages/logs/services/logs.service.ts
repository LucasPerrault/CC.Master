import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import {
  defaultPagingParams,
  IApiV3PagingParams,
  IApiV3SortParams,
  IHttpApiV3CollectionCountResponse,
  IHttpApiV3Response,
  IHttpQueryParams,
} from '../../../common/queries';
import { IEnvironmentLog } from '../models';

@Injectable()
export class LogsService {
  private logs: BehaviorSubject<IEnvironmentLog[]> = new BehaviorSubject<IEnvironmentLog[]>([]);
  private numberOfTotalLogs: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  private isShownMoreDataLoading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private isRefreshedDataLoading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private httpClient: HttpClient) {}

  public async refreshLogsAsync(sortParams: IApiV3SortParams, params?: IHttpQueryParams): Promise<void> {
    this.isRefreshedDataLoading.next(true);

    await this.updateLogsAsync(
      defaultPagingParams,
      sortParams,
      params,
    );

    this.isRefreshedDataLoading.next(false);
  }

  public async showMoreDataAsync(sortParam: IApiV3SortParams, params?: IHttpQueryParams): Promise<void> {
    if (this.isShownMoreDataLoading.value) {
      return;
    }

    if (this.logs.value.length >= this.numberOfTotalLogs.value) {
      return;
    }

    const pagingParams = {
      limit: defaultPagingParams.limit,
      skip: this.logs.value.length,
    };

    this.isShownMoreDataLoading.next(true);
    await this.updateLogsAsync(pagingParams, sortParam, params);
    this.isShownMoreDataLoading.next(false);
  }

  private async updateLogsAsync(
    paginationParams: IApiV3PagingParams,
    sortParams: IApiV3SortParams,
    params?: IHttpQueryParams,
  ): Promise<void> {
    if (!paginationParams.skip) {
      this.logs.next([]);
    }

    const response = await this.getLogs$(paginationParams, sortParams, params).toPromise();

    this.logs.next([...this.logs.value, ...response.items]);
    this.numberOfTotalLogs.next(response.count);
  }

  private getLogs$(
    pagingParams: IApiV3PagingParams,
    sortParams: IApiV3SortParams,
    queryParams?: IHttpQueryParams,
  ): Observable<IHttpApiV3CollectionCountResponse<IEnvironmentLog>> {
    const fields = 'collection.count,id,name,user,isAnonymizedData,activity,createdOn,environment[subDomain,domainName],' +
      'messages[id,message,type]';
    const environmentLogUrl = `/api/v3/environmentLogs`;

    return this.httpClient.get<IHttpApiV3Response<IHttpApiV3CollectionCountResponse<IEnvironmentLog>>>(environmentLogUrl, {
      params: {
        ...queryParams,
        fields,
        paging: `${pagingParams.skip},${pagingParams.limit}`,
        orderBy: `${sortParams.field},${sortParams.order}`,
      },
    }).pipe(map(response => response.data));
  }

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this.logs.asObservable();
  }

  public get isShownMoreDataLoading$(): Observable<boolean> {
    return this.isShownMoreDataLoading.asObservable();
  }

  public get isRefreshedDataLoading$(): Observable<boolean> {
    return this.isRefreshedDataLoading.asObservable();
  }
}
