import { IContact } from '../common/models/contact.interface';

export interface IGenericContact extends IContact {
  roleId: number;
  roleCode: string;
}
