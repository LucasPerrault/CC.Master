import { IPriceList } from '@cc/domain/billing/offers';

export type IEditablePriceGrid = Omit<IPriceList, 'id'>;
