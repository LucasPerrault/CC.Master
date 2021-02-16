import { HttpParams } from '@angular/common/http';
import { ApiV3Order, IApiV3SortParams } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';

export const toApiV3SortParams = (sortParams: ISortParams[]): IApiV3SortParams[] =>
  sortParams.map(param => ({
    field: param.field,
    order: getApiV3OrderPrefix(param.order),
  }));

export const getApiV3OrderPrefix = (order: SortOrder): ApiV3Order =>
  order === SortOrder.Asc ? 'asc' : 'desc';


export const apiV3SortToHttpParams = (params: HttpParams, sortParams: IApiV3SortParams[]): HttpParams => {
  const formattedParams = sortParams.map(sort => `${sort.field},${sort.order}`);
  return params.set('orderBy', formattedParams.join(','));
};



