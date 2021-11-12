import { IEstablishment } from './establishment.interface';

export interface ILegalUnit {
  id: number;
  environmentId: number;
  name: string;
  code: string;
  legalIdentificationNumber: string;
  activityCode: string;
  countryId: number;
  headQuartersId: number;
  createdAt: string;
  isArchived: boolean;

  country: ICountry;
  establishments: IEstablishment[];
}

export interface ICountry {
  id: number;
  name: string;
}
