import { IAppInstance } from '../../environments/models/app-instance.interface';
import { IContact } from '../common/models/contact.interface';

export interface IAppContact extends IContact {
  appInstanceId: number;
  appInstance: IAppInstance;
}
