import { EstablishmentType } from '../constants/establishment-type.enum';
import { IContractCount } from './contract-count.interface';
import { IEstablishmentContract } from './establishment-contract.interface';

export interface IEstablishmentActionsContext {
  establishmentType: EstablishmentType;
  contract: IEstablishmentContract;
  realCounts: IContractCount[];
  lastCountPeriod: Date;
}
