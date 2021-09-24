import { ILuUser } from '@lucca-front/ng/user';

export interface IContactUser extends ILuUser {
  id: number;
  firstName: string;
  lastName: string;
  picture?: {
    href: string;
  };
  mail?: string;
  culture: any;
}
