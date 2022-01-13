export enum CountAdditionalColumn {
  Client = 'client',
  Distributor = 'distributor',
  Offer = 'offer',
  CountPeriod = 'countPeriod',
  NormalCount = 'normalCount',
  EnhancedCount = 'enhancedCount',
  MinimalBilling = 'minimalBilling',
  UnitPrice = 'unitPrice',
  TotalBillable = 'totalBillable',
  TotalPartner = 'totalPartner',
  TotalLucca = 'totalLucca',
  ClientRebate = 'clientRebate',
  DistributorRebate = 'distributorRebate',
}

export interface ICountAdditionalColumn {
  id: CountAdditionalColumn;
  name: string;
}

export const countAdditionalColumns: ICountAdditionalColumn[] = [
  {
    id: CountAdditionalColumn.Offer,
    name: 'counts_column_selection_offer',
  },
  {
    id: CountAdditionalColumn.CountPeriod,
    name: 'counts_column_selection_countPeriod',
  },
  {
    id: CountAdditionalColumn.NormalCount,
    name: 'counts_column_selection_normalCount',
  },
  {
    id: CountAdditionalColumn.EnhancedCount,
    name: 'counts_column_selection_enhancedCount',
  },
  {
    id: CountAdditionalColumn.MinimalBilling,
    name: 'counts_column_selection_minimalBilling',
  },
  {
    id: CountAdditionalColumn.UnitPrice,
    name: 'counts_column_selection_unitPrice',
  },
  {
    id: CountAdditionalColumn.TotalBillable,
    name: 'counts_column_selection_totalBillable',
  },
  {
    id: CountAdditionalColumn.TotalPartner,
    name: 'counts_column_selection_totalPartner',
  },
  {
    id: CountAdditionalColumn.TotalLucca,
    name: 'counts_column_selection_totalLucca',
  },
  {
    id: CountAdditionalColumn.ClientRebate,
    name: 'counts_column_selection_clientRebate',
  },
  {
    id: CountAdditionalColumn.DistributorRebate,
    name: 'counts_column_selection_distributorRebate',
  },
];

export const defaultColumnsDisplayed = [
  CountAdditionalColumn.Offer,
  CountAdditionalColumn.CountPeriod,
  CountAdditionalColumn.NormalCount,
  CountAdditionalColumn.EnhancedCount,
  CountAdditionalColumn.MinimalBilling,
  CountAdditionalColumn.UnitPrice,
  CountAdditionalColumn.TotalBillable,
  CountAdditionalColumn.TotalPartner,
  CountAdditionalColumn.TotalLucca,
  CountAdditionalColumn.ClientRebate,
  CountAdditionalColumn.DistributorRebate,
].map(c => countAdditionalColumns.find(column => column.id === c));
