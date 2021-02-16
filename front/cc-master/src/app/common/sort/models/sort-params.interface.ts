import { SortOrder } from '../enums/sort-order.enum';

export interface ISortParams {
  field: string;
  order: SortOrder;
}
