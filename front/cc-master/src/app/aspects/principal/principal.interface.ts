import { IPermission } from '../rights';

export interface IPrincipal {
  id: number;
  name: string;
  permissions: IPermission[];
  culture: ICulture;
  distributorId: number;
  isLucca: boolean;
}

export interface ICulture {
  code: string;
}
