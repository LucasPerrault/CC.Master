export enum ContractAdditionalColumn {
  Distributor = 'distributor',
  Vintage = 'vintage',
  Product = 'product',
  Offer = 'offer',
  Status = 'status',
  StartAt = 'startAt',
  CloseOn = 'closeOn',
  CreatedOn = 'createdOn',
  BillingEntity = 'billingEntity',
  Establishments = 'establishments',
  EstablishmentsHealth = 'establishmentsHealth',
}

export interface IContractAdditionalColumn {
  id: ContractAdditionalColumn;
  name: string;
}

export const defaultColumnsDisplayed = [
  ContractAdditionalColumn.Distributor,
  ContractAdditionalColumn.Vintage,
  ContractAdditionalColumn.Product,
  ContractAdditionalColumn.Status,
  ContractAdditionalColumn.EstablishmentsHealth,
];

export const contractAdditionalColumns: IContractAdditionalColumn[] = [
  {
    id: ContractAdditionalColumn.Distributor,
    name: 'front_contractPage_column_distributor',
  },
  {
    id: ContractAdditionalColumn.Vintage,
    name: 'front_contractPage_column_vintage',
  },
  {
    id: ContractAdditionalColumn.Product,
    name: 'front_contractPage_column_product',
  },
  {
    id: ContractAdditionalColumn.Offer,
    name: 'front_contractPage_column_offer',
  },
  {
    id: ContractAdditionalColumn.Status,
    name: 'front_contractPage_column_status',
  },
  {
    id: ContractAdditionalColumn.StartAt,
    name: 'front_contractPage_column_startAt',
  },
  {
    id: ContractAdditionalColumn.CloseOn,
    name: 'front_contractPage_column_endAt',
  },
  {
    id: ContractAdditionalColumn.CreatedOn,
    name: 'front_contractPage_column_createdAt',
  },
  {
    id: ContractAdditionalColumn.Establishments,
    name: 'front_contractPage_column_establishments',
  },
  {
    id: ContractAdditionalColumn.BillingEntity,
    name: 'billingEntity_column',
  },
  {
    id: ContractAdditionalColumn.EstablishmentsHealth,
    name: 'front_contractPage_column_establishmentsHealth',
  },
];
