import { IPriceRowForm } from './price-list-form.interface';

export interface IPriceListEditionDto {
  id: number;
  startsOn: string;
  rows: IPriceRowForm[];
}

export interface IPriceRowEditionDto {
  maxIncludedCount: number;
  unitPrice: number;
  fixedPrice: number;
  listId: number;
}
