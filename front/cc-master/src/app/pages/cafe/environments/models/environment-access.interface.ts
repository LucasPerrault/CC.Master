import { IDistributor } from '@cc/domain/billing/distributors';

export interface IEnvironmentAccess {
  id: number;
  environmentId: number;
  distributorId: number;
  distributor: IDistributor;
}
