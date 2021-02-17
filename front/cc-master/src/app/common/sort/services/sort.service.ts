import { Injectable } from '@angular/core';

import { SortOrder } from '../enums/sort-order.enum';
import { ISortParams } from '../models/sort-params.interface';

@Injectable()
export class SortService {
  public isSorted(order: SortOrder, field: string, sortParams: ISortParams[]): boolean {
    const sortParam = sortParams.find(s => s.field === field);
    if (!sortParam) {
      return false;
    }

    return sortParam.field === field && sortParam.order === order;
  }

  public updateSortParam(field: string, order: SortOrder, sortParams: ISortParams[]): ISortParams[] {
    const sortParamsFiltered = sortParams.filter(s => s.field !== field);
    const orderUpdated = this.toggleSortOrder(sortParams, field, order);

    const sortParamUpdated = { field, order: orderUpdated };
    return [sortParamUpdated, ...sortParamsFiltered];
  }

  private toggleSortOrder(sortParams: ISortParams[], field: string, order: SortOrder): SortOrder {
    const sortParam = sortParams.find(s => s.field === field);
    if (!sortParam) {
      return order;
    }

    return sortParam.order === SortOrder.Asc ? SortOrder.Desc : SortOrder.Asc;
  }
}
