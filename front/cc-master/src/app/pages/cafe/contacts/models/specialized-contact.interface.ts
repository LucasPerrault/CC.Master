import { IContact } from './contact.interface';

export interface ISpecializedContact extends IContact {
  roleId: number;
  roleCode: string;
}
