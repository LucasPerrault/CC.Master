export enum CountsQueryParamsKey {
  CountPeriod = 'countPeriod',
  ClientId = 'clientId',
  DistributorId = 'distributorId',
  OfferId = 'offerId',
  EnvironmentGroupId = 'environmentGroupId',
  ProductId = 'productId',
  Column = 'column',
  EnvironmentId = 'environmentId',
}

export type ICountsQueryParams = {
  [key in CountsQueryParamsKey]: string;
};

