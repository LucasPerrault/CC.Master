import { IPriceRow } from '@cc/domain/billing/offers';

import { IPriceRowForm } from './price-list-form.interface';

export interface IPriceListEditionDto {
  id: number;
  offerId: number;
  startsOn: string;
  rows: IPriceRowEditionDto[];
}

export type IPriceRowEditionDto = IPriceRow | IPriceRowForm;
