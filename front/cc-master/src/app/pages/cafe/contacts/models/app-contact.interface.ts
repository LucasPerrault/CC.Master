import { IAppInstance } from '../../instance/models/app-instance.interface';
import { IContact } from './contact.interface';

export interface IAppContact extends IContact {
  appInstanceId: number;
  appInstance: IAppInstance;
}
