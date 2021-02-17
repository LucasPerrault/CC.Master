import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginatedListState } from '@cc/common/paging/enums/paginated-list-state.enum';
import { apiV3SortToHttpParams,IApiV3SortParams } from '@cc/common/queries';
import { BehaviorSubject, Observable } from 'rxjs';

import { IPaginatedResult } from '../models/paginated-result.interface';
import { defaultPagingParams, IPagingParams } from '../models/paging-params.interface';

type PagedFetchFunction<T> = (httpParams: HttpParams) => Observable<IPaginatedResult<T>>;

@Injectable()
export class PagingService {
  public paginate<T>(fetch: PagedFetchFunction<T>): PaginatedList<T> {
    return new PaginatedList<T>(fetch);
  }
}

export class PaginatedList<T> {

  private paging: IPagingParams;
  private sortParams: IApiV3SortParams[];
  private filterParams: HttpParams;
  private state: BehaviorSubject<PaginatedListState> = new BehaviorSubject<PaginatedListState>(PaginatedListState.Idle);

  private items: BehaviorSubject<T[]> = new BehaviorSubject([]);
  private totalCount: BehaviorSubject<number> = new BehaviorSubject(0);

  public get items$(): Observable<T[]> {
    return this.items.asObservable();
  }

  public get totalCount$(): Observable<number> {
    return this.totalCount.asObservable();
  }

  public get state$(): Observable<PaginatedListState> {
    return this.state.asObservable();
  }

  constructor(private fetchMore: PagedFetchFunction<T>, paging: IPagingParams = defaultPagingParams) {
    this.paging = paging;
  }

  public showMore(): void {
    if (this.state.value !== PaginatedListState.Idle && this.state.value !== PaginatedListState.Error) {
      return;
    }

    if (this.items.value.length >= this.totalCount.value) {
      return;
    }

    this.paging.skip = this.items.value.length;

    this.update(PaginatedListState.LoadMore);
  }

  public updateFilters(filter: HttpParams): void {
    this.filterParams = filter;

    this.resetPaging();
    this.update(PaginatedListState.UpdateFilter);
  }

  public updateSort(sort: IApiV3SortParams[]): void {
    this.sortParams = sort;

    this.resetPaging();
    this.update(PaginatedListState.UpdateSort);
  }

  private resetPaging(): void {
    this.paging.skip = 0;
  }

  private update(state: PaginatedListState): void {
    this.state.next(state);

    if (!this.paging.skip) {
      this.items.next([]);
    }

    const params = !!this.filterParams ? this.filterParams : new HttpParams();
    const paramsWithSort = apiV3SortToHttpParams(params, this.sortParams);
    const paramsWithPaging = this.toApiV3PagingParams(paramsWithSort, this.paging);

    this.fetchMore(paramsWithPaging).subscribe(
      res => {
        this.items.next([...this.items.value, ...res.items]);
        this.totalCount.next(res.totalCount);
        this.state.next(PaginatedListState.Idle);
      },
      e => this.state.next(PaginatedListState.Error),
    );
  }

  private toApiV3PagingParams(params: HttpParams, paging: IPagingParams): HttpParams {
    return params.set('paging', `${this.paging.skip},${this.paging.limit}`);
  }
}
