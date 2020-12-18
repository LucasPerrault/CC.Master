import {BehaviorSubject, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {
  defaultPagingParams,
  IApiV3PagingParams,
  IApiV3SortParams,
  IHttpApiV3CollectionCountResponse,
  IHttpApiV3Response,
  IHttpQueryParams
} from '../queries';
import {IEnvironmentLog} from '../models';

@Injectable()
export class LogsService {
  private _logs$: BehaviorSubject<IEnvironmentLog[]> = new BehaviorSubject<IEnvironmentLog[]>([]);
  private _numberOfTotalLogs$: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  private _isShownMoreDataLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private _isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private _httpClient: HttpClient) {}

  public async refreshLogsAsync(sortParams: IApiV3SortParams, params?: IHttpQueryParams): Promise<void> {
    this._isLoading$.next(true);

    await this.updateLogsAsync(
      defaultPagingParams,
      sortParams,
      params
    );

    this._isLoading$.next(false);
  }

  public async showMoreDataAsync(sortParam: IApiV3SortParams, params?: IHttpQueryParams): Promise<void> {
    if (this._isShownMoreDataLoading$.value) {
      return;
    }

    if (this._logs$.value.length >= this._numberOfTotalLogs$.value) {
      return;
    }

    const pagingParams = {
      limit: defaultPagingParams.limit,
      skip: this._logs$.value.length,
    }

    this._isShownMoreDataLoading$.next(true);
    await this.updateLogsAsync(pagingParams, sortParam, params);
    this._isShownMoreDataLoading$.next(false);
  }

  private async updateLogsAsync(
    paginationParams: IApiV3PagingParams,
    sortParams: IApiV3SortParams,
    params?: IHttpQueryParams
  ): Promise<void> {
    if (!paginationParams.skip) {
      this._logs$.next([]);
    }

    const response = await this.getLogs$(paginationParams, sortParams, params).toPromise();

    this._logs$.next([...this._logs$.value, ...response.items]);
    this._numberOfTotalLogs$.next(response.count);
  }

  private getLogs$(
    pagingParams: IApiV3PagingParams,
    sortParams: IApiV3SortParams,
    queryParams?: IHttpQueryParams,
  ): Observable<IHttpApiV3CollectionCountResponse<IEnvironmentLog>> {
    const fields = 'collection.count,id,name,user,isAnonymizedData,activity,createdOn,environment[subDomain,domainName],messages[id,message,type]';
    const environmentLogUrl = `/api/v3/environmentLogs`;

    return this._httpClient.get<IHttpApiV3Response<IHttpApiV3CollectionCountResponse<IEnvironmentLog>>>(environmentLogUrl, {
      params: {
        ...queryParams,
        fields: fields,
        paging: `${pagingParams.skip},${pagingParams.limit}`,
        orderBy: `${sortParams.field},${sortParams.order}`
      }
    }).pipe(map(response => response.data));
  }

  public get logs$(): Observable<IEnvironmentLog[]> {
    return this._logs$.asObservable();
  }

  public get isLoading$(): Observable<boolean> {
    return this._isLoading$.asObservable();
  }
}
