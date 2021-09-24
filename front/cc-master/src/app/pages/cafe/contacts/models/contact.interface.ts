import { IEnvironment } from '../../instance/models/environment.interface';
import { IEstablishment } from '../../instance/models/establishment.interface';
import { IContactUser } from './contact-user.interface';
import { IContactWarning } from './contact-warning.interface';

export interface IContact {
  id: number;
  userId: number;
  environmentId: number;
  establishmentId: number;
  createdAt: string;
  expiresAt: string;
  isConfirmed: boolean;
  warnings: IContactWarning[];

  user: IContactUser;
  environment: IEnvironment;
  establishment: IEstablishment;
}
