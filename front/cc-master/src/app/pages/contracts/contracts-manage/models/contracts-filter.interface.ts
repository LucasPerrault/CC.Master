import { IDateRange } from '@cc/common/date';
import { IClient } from '@cc/domain/billing/clients';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEstablishment } from '@cc/domain/billing/establishments';
import { IOffer, IProduct } from '@cc/domain/billing/offers';
import { IEnvironment } from '@cc/domain/environments';

import { DistributorFilter } from '../../common/distributor-filter-button-group';
import { IContractEstablishmentHealth } from './contract-establishment-health.interface';
import { IContractState } from './contract-state.interface';

export interface IContractsFilter {
  ids: string[];
  name: string;
  distributorFilter: DistributorFilter;
  clients: IClient[];
  distributors: IDistributor[];
  products: IProduct[];
  offers: IOffer[];
  environments: IEnvironment[];
  establishments: IEstablishment[];
  states: IContractState[];
  establishmentHealth: IContractEstablishmentHealth;
  createdAt: Date;
  periodOn: IDateRange;
}
