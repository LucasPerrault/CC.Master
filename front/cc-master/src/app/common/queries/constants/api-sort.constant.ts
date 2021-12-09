import { HttpParams } from '@angular/common/http';
import { ApiStandard } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';

interface IApiSortParams {
  field: string;
  order: ApiV3Order | ApiV4Order;
}

type ApiV3Order = 'asc' | 'desc';
type ApiV4Order = '+' | '-' | '';

export class ApiSortHelper {
  public static v3SortKey = 'orderBy';
  public static v3AscKey: ApiV3Order = 'asc';
  public static v3DscKey: ApiV3Order = 'desc';
  public static v4SortKey = 'sort';
  public static defaultV4AscKey: ApiV4Order = '';
  public static v4AscKey: ApiV4Order = '+';
  public static v4DscKey: ApiV4Order = '-';

  public static toV4HttpParams(params: HttpParams, sortParams: ISortParams, withoutAscSymbol: boolean = true): HttpParams {
    const apiSortParams = ApiSortHelper.toApiSortParams(sortParams, ApiStandard.V4, withoutAscSymbol);
    return !!sortParams
      ? params.set(ApiSortHelper.v4SortKey, `${apiSortParams.order}${apiSortParams.field}`)
      : params;
  }

  public static toV3HttpParams(params: HttpParams, sortParams: ISortParams): HttpParams {
    const apiSortParams = ApiSortHelper.toApiSortParams(sortParams, ApiStandard.V3);
    return !!sortParams
      ? params.set(ApiSortHelper.v3SortKey, `${apiSortParams.field},${apiSortParams.order}`)
      : params;
  }

  private static toApiSortParams(sortParams: ISortParams, standard: ApiStandard, withoutAscSymbol?: boolean): IApiSortParams {
    return {
      field: sortParams.field,
      order: ApiSortHelper.getApiOrderPrefix(sortParams.order, standard, withoutAscSymbol),
    };
  };

  private static getApiOrderPrefix(order: SortOrder, standard: ApiStandard, withoutAscSymbol?: boolean): ApiV3Order | ApiV4Order {
    switch (standard) {
      case ApiStandard.V3:
        return order === SortOrder.Asc ? ApiSortHelper.v3AscKey : ApiSortHelper.v3DscKey;
      case ApiStandard.V4:
        return order === SortOrder.Asc
          ? (withoutAscSymbol ? ApiSortHelper.defaultV4AscKey : ApiSortHelper.v4AscKey)
          : ApiSortHelper.v4DscKey;
    }
  }
}


