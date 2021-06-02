export interface IHttpApiV3Response<T> {
  data: T;
}

export interface IHttpApiV3Count {
  count: number;
}

export interface IHttpApiV3Collection<T> {
  items: T[];
}

export interface IHttpApiV3CollectionCount<T> extends IHttpApiV3Collection<T> {
  count: number;
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IHttpApiV3CountResponse extends IHttpApiV3Response<IHttpApiV3Count> {}
// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IHttpApiV3CollectionResponse<T> extends IHttpApiV3Response<IHttpApiV3Collection<T>> {}
// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IHttpApiV3CollectionCountResponse<T> extends IHttpApiV3Response<IHttpApiV3CollectionCount<T>> {}



