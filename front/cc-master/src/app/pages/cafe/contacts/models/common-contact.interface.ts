import { IContact } from './contact.interface';

export interface ICommonContact extends IContact {
  roleId: number;
  roleCode: string;
}
