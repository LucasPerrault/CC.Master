export enum OffersQueryParamsKey {
  Search = 'search',
  Tag = 'tag',
  ProductId = 'productId',
  Currency = 'currency',
  BillingMode = 'billingMode',
  State = 'state'
}

export type IOfferQueryParams = {
  [key in OffersQueryParamsKey]: string;
};
