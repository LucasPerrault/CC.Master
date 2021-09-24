import { IClient } from '../../billing/models/client.interface';
import { IContact } from './contact.interface';

export interface IClientContact extends IContact {
  id: number;
  roleId: number;
  roleCode: string;
  clientId: number;

  client: IClient;
}
