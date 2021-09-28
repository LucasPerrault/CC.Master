import { IContact } from '../common/models/contact.interface';

export interface ISpecializedContact extends IContact {
  roleId: number;
  roleCode: string;
}
