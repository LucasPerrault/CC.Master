export interface IHttpApiV4CollectionResponse<T> {
  items: T[];
}

export interface IHttpApiV4CollectionCountResponse<T> extends IHttpApiV4CollectionResponse<T> {
  count: number;
}
