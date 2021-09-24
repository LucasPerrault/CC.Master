import { IEstablishment } from '../../instance/models/establishment.interface';

export interface IEstablishmentContract {
  contractId: number;
  establishmentId: number;
  environmentId: number;

  establishment: IEstablishment;
}
