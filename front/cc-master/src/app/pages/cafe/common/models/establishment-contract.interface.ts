import { IEstablishment } from './establishment.interface';

export interface IEstablishmentContract {
  contractId: number;
  establishmentId: number;
  environmentId: number;

  establishment: IEstablishment;
}
