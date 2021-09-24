import { IAppInstance } from './app-instance.interface';
import { ILegalUnit } from './legal-unit.interface';

export interface IEnvironment {
  id: number;
  subdomain: string;
  domain: string;
  isActive: boolean;
  createdAt: string;
  productionHost: string;

  legalUnits: ILegalUnit[];
  appInstances: IAppInstance[];
}
