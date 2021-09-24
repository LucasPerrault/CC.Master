import { IEnvironment } from '../../../instance/models/environment.interface';

export interface IContactListEntry {
  id: number;
  lastname: string;
  firstname: string;
  role: string;
  mail: string;
  createdAt: string;
  expiredAt: string;
  isConfirmed: boolean;
  environmentId: number;
  environment: IEnvironment;
}
