export interface IPagingParams {
  page: number;
  limit: number;
}

export const defaultPagingParams: IPagingParams = {
  page: 0,
  limit: 25,
};
