import { IPriceRowForm } from './price-list-form.interface';

export interface IPriceListCreationDto {
  startsOn: string;
  rows: IPriceRowForm[];
}
