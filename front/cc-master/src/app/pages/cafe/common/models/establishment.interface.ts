import { IEnvironment } from './environment.interface';
import { IEstablishmentFacetValue } from './facet-value.interface';
import { ILegalUnit } from './legal-unit.interface';

export interface IEstablishment {
  id: number;
  name: string;
  code: string;
  legalUnitId: number;
  legalIdentificationNumber: string;
  activityCode: string;
  timeZoneId: string;
  usersCount: number;
  createdAt: string;
  isArchived: boolean;
  environmentId: number;

  environment: IEnvironment;
  legalUnit: ILegalUnit;
  facets: IEstablishmentFacetValue[];
}
