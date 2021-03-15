import { Injectable } from '@angular/core';

import { SortOrder } from '../enums/sort-order.enum';
import { ISortParams } from '../models/sort-params.interface';

@Injectable()
export class SortService {
  public isSorted(order: SortOrder, field: string, sortParams: ISortParams): boolean {
    if (!sortParams) {
      return false;
    }

    return sortParams.field === field && sortParams.order === order;
  }

  public updateSortParam(field: string, order: SortOrder, sortParams: ISortParams): ISortParams {
    const orderUpdated = this.toggleSortOrder(sortParams, order);
    return { field, order: orderUpdated };
  }

  private toggleSortOrder(sortParams: ISortParams, order: SortOrder): SortOrder {
    if (!sortParams) {
      return order;
    }

    return sortParams.order === SortOrder.Asc ? SortOrder.Desc : SortOrder.Asc;
  }
}
