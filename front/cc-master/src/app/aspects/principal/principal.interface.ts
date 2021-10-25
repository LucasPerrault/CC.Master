import { IPermission } from '../rights';

export class DistributorIds {
	public static readonly lucca = 37;
	public static isLuccaUser = (user: IPrincipal) => user.distributorId === DistributorIds.lucca;
}

export interface IPrincipal {
	id: number;
	name: string;
	permissions: IPermission[];
	culture: ICulture;
	distributorId: number;
}

export interface ICulture {
  code: string;
}
