import { IEnvironment } from '../../../environments/models/environment.interface';
import { IEstablishment } from '../../../environments/models/establishment.interface';
import { IContactWarning } from './contact-warning.interface';

export interface IContact {
  id: number;
  userId: number;
  userFirstName: string;
  userLastName: string;
  userMail: string;
  environmentId: number;
  establishmentId: number;
  createdAt: string;
  expiresAt: string;
  isConfirmed: boolean;
  warnings: IContactWarning[];

  environment: IEnvironment;
  establishment: IEstablishment;
}
