import { IPriceRow } from '@cc/domain/billing/offers';

import { IPriceRowCreationDto } from './price-list-creation-dto.interface';

export interface IPriceListEditionDto {
  id: number;
  offerId: number;
  startsOn: string;
  rows: IPriceRowEditionDto[];
}

export type IPriceRowEditionDto = IPriceRow | IPriceRowCreationDto;
