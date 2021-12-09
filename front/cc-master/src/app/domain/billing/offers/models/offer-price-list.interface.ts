import { IPriceRow } from './offer-price-row.interface';

export interface IPriceList {
  id: number;
  offerId: number;
  startsOn: string;
  rows: IPriceRow[];
}
