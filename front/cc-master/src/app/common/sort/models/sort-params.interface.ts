export interface ISortParams {
  field: string;
  order: SortOrder;
}

export type SortOrder = 'asc' | 'desc';
