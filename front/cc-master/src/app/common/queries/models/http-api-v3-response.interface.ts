export interface IHttpApiV3Response<T> {
  data: T;
}

export interface IHttpApiV3Collection<T> {
  items: T[];
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IHttpApiV3CollectionResponse<T> extends IHttpApiV3Response<IHttpApiV3Collection<T>> {}
// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IHttpApiV3CollectionCountResponse<T> extends IHttpApiV3Response<IHttpApiV3CollectionCount<T>> {}


export interface IHttpApiV3CollectionCount<T> extends IHttpApiV3Collection<T> {
  count: number;
}

