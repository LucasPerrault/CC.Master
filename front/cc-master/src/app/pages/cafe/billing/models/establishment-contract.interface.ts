import { IEstablishment } from '../../environments/models/establishment.interface';

export interface IEstablishmentContract {
  contractId: number;
  establishmentId: number;
  environmentId: number;

  establishment: IEstablishment;
}
