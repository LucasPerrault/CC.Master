import {IPermission} from '../rights';

export interface IPrincipal {
	id: number;
	name: string;
	isLuccaUser: boolean;
	departmentCode: string;
	permissions: IPermission[];
}
