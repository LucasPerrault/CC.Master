import { ICount } from '@cc/domain/billing/counts';

import { EstablishmentType } from '../constants/establishment-type.enum';
import { IEstablishmentContract } from './establishment-contract.interface';

export interface IEstablishmentActionsContext {
  establishmentType: EstablishmentType;
  contract: IEstablishmentContract;
  realCounts: ICount[];
  lastCountPeriod: Date;
}
