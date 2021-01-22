export interface IPagingParams {
  skip: number;
  limit: number;
}

export const defaultPagingParams: IPagingParams = {
  skip: 0,
  limit: 25,
};
