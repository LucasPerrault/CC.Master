export interface IHttpApiV3Response<T> {
  data: T;
}

export interface IHttpApiV3Collection<T> {
  items: T[];
}

export interface IHttpApiV3CollectionCountResponse<T> extends IHttpApiV3Collection<T> {
  count: number;
}

