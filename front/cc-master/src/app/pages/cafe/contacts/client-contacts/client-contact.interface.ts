import { IClient } from '../../common/models/client.interface';
import { IContact } from '../common/models/contact.interface';

export interface IClientContact extends IContact {
  id: number;
  roleId: number;
  roleCode: string;
  clientId: number;

  client: IClient;
}
