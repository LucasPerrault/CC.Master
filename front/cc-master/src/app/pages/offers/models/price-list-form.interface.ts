export interface IPriceRowForm {
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
}

export interface IPriceListForm {
  startsOn: Date;
  rows: IPriceRowForm[];
}
