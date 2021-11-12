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
}
