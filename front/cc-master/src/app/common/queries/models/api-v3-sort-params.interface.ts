export interface IApiV3SortParams {
  field: string;
  order: ApiV3Order;
}

export type ApiV3Order = 'asc' | 'desc'

