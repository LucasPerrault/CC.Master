import { IAppInstance } from './app-instance.interface';
import { IEnvironmentAccess } from './environment-access.interface';
import { ILegalUnit } from './legal-unit.interface';

export interface IEnvironment {
  id: number;
  subdomain: string;
  domain: string;
  isActive: boolean;
  createdAt: string;
  productionHost: string;
  cluster: string;
  accesses: IEnvironmentAccess[];

  legalUnits: ILegalUnit[];
  appInstances: IAppInstance[];
}
