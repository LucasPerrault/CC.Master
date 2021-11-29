import { IPriceRow } from '@cc/domain/billing/offers';

import { IPriceRowCreationDto } from './price-list-creation-dto.interface';

export interface IPriceListEditionDto {
  id: number;
  offerId: number;
  startsOn: string;
  rows: IPriceRowEditionDto[];
}

export interface IPriceRowCreationDtoDuringEdition extends IPriceRowCreationDto {
  listId: number;
}


export type IPriceRowEditionDto = IPriceRow | IPriceRowCreationDtoDuringEdition;
