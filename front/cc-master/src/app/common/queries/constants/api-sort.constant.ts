import { HttpParams } from '@angular/common/http';
import { ApiV3Order, IApiV3SortParams } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';

export const apiV3SortKey = 'orderBy';
export const apiV3SortOrderAscendingKey = 'asc';
export const apiV3SortOrderDescendingKey = 'desc';

export const toApiV3SortParams = (sortParams: ISortParams): IApiV3SortParams => ({
    field: sortParams.field,
    order: getApiV3OrderPrefix(sortParams.order),
  });

export const getApiV3OrderPrefix = (order: SortOrder): ApiV3Order =>
  order === SortOrder.Asc ? apiV3SortOrderAscendingKey : apiV3SortOrderDescendingKey;


export const apiV3SortToHttpParams = (params: HttpParams, sortParams: IApiV3SortParams): HttpParams => {
  if (!sortParams) {
    return params;
  }

  const formattedParams = `${sortParams.field},${sortParams.order}`;
  return params.set(apiV3SortKey, formattedParams);
};



