import { ICount } from '@cc/domain/billing/counts';

import { IEstablishmentContract } from './establishment-contract.interface';
import { IListEntry } from './establishment-list-entry.interface';

export interface IEstablishmentActionsContext {
  allEntries: IListEntry[];
  contract: IEstablishmentContract;
  realCounts: ICount[];
  lastCountPeriod: Date;
}
