import { IDistributor } from '@cc/domain/billing/distributors/v4';

import { IDemo } from './demo.interface';

export enum DemoFormKey {
  Subdomain = 'subdomain',
  Source = 'source',
  Password = 'password',
  Distributor = 'distributor',
  Comment = 'comment',
}

export interface IDemoCreationForm {
  subdomain: string;
  source: IDemo;
  password: string;
  distributor: IDistributor;
  comment: string;
}

export class IDemoDuplication {
  public id: number;
  public instanceDuplicationId: string;
}

export class IDemoCreationDto {
  public sourceId: number;
  public subdomain: string;
  public comment: string;
  public password: string;
  public distributorCode: string;
}
