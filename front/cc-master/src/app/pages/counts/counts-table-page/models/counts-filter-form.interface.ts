import { IDateRange } from '@cc/common/date';
import { IClient } from '@cc/domain/billing/clients';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct } from '@cc/domain/billing/offers';
import { IEnvironment, IEnvironmentGroup } from '@cc/domain/environments';

export enum CountsFilterFormKey {
  CountPeriod = 'countPeriod',
  Distributors = 'distributors',
  Offers = 'offers',
  EnvironmentGroups = 'environmentGroups',
  Products = 'products',
  Clients = 'clients',
  Environments = 'environments',
}

export interface ICountsFilterForm {
  countPeriod: IDateRange;
  distributors: IDistributor[];
  offers: IOffer[];
  environmentGroups: IEnvironmentGroup[];
  products: IProduct[];
  clients: IClient[];
  environments: IEnvironment[];
}
