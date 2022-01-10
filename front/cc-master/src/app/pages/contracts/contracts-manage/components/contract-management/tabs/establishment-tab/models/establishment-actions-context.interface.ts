import { ICount } from '@cc/domain/billing/counts';

import { IEstablishmentContract } from './establishment-contract.interface';

export interface IEstablishmentActionsContext {
  contract: IEstablishmentContract;
  realCounts: ICount[];
  lastCountPeriod: Date;
}
