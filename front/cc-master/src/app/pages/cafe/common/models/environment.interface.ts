import { IAppInstance } from '../../environments/models/app-instance.interface';
import { IEnvironmentAccess } from '../../environments/models/environment-access.interface';
import { IEnvironmentFacetValue } from './facet-value.interface';
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
  distributorType: DistributorType;

  legalUnits: ILegalUnit[];
  appInstances: IAppInstance[];
  facets: IEnvironmentFacetValue[];
}

export enum DistributorType {
  DirectOnly = 'DirectOnly',
  IndirectOnly = 'IndirectOnly',
  DirectAndIndirect = 'DirectAndIndirect',
}
