import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginatedListState } from '@cc/common/paging/enums/paginated-list-state.enum';
import { ApiStandard } from '@cc/common/queries';
import { BehaviorSubject, Observable } from 'rxjs';

import { IPaginatedResult } from '../models/paginated-result.interface';
import { defaultPagingParams, IPagingParams } from '../models/paging-params.interface';

type PagedFetchFunction<T> = (httpParams: HttpParams) => Observable<IPaginatedResult<T>>;

@Injectable()
export class PagingService {
  public paginate<T>(
    fetch: PagedFetchFunction<T>,
    paging: IPagingParams = defaultPagingParams,
    standard: ApiStandard = ApiStandard.V3,
  ): PaginatedList<T> {
    return new PaginatedList<T>(fetch, paging, standard);
  }
}

export class PaginatedList<T> {

  private paging: IPagingParams;
  private readonly standard: ApiStandard;
  private httpParams: HttpParams;
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

  constructor(private fetchMore: PagedFetchFunction<T>, paging, standard) {
    this.paging = paging;
    this.standard = standard;
  }

  public nextPage(): void {
    if (this.state.value !== PaginatedListState.Idle && this.state.value !== PaginatedListState.Error) {
      return;
    }

    if (this.items.value.length >= this.totalCount.value) {
      return;
    }

    this.paging.page = this.paging.page + 1;

    this.update(PaginatedListState.LoadMore);
  }

  public updateHttpParams(httpParams: HttpParams): void {
    this.httpParams = httpParams;

    this.resetPaging();
    this.update(PaginatedListState.Update);
  }

  private resetPaging(): void {
    this.paging.page = 0;
  }

  private update(state: PaginatedListState): void {
    this.state.next(state);

    if (!this.paging.page) {
      this.items.next([]);
    }

    const paramsWithPaging = this.toPagingParams(this.httpParams, this.paging);

    this.fetchMore(paramsWithPaging).subscribe(
      res => {
        const previousValues = !!this.paging.page ? this.items.value : [];
        this.items.next([...previousValues, ...res.items]);

        this.totalCount.next(res.totalCount);
        this.state.next(PaginatedListState.Idle);
      },
      e => this.state.next(PaginatedListState.Error),
    );
  }

  private toPagingParams(params: HttpParams, paging: IPagingParams): HttpParams {
    switch (this.standard) {
      case ApiStandard.V3:
        return params.set('paging', `${paging.page * paging.limit},${paging.limit}`);
      case ApiStandard.V4:
        return params.set('page', paging.page + 1).set('limit', paging.limit);
    }
  }
}
