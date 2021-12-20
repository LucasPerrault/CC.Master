import { IDistributor } from '@cc/domain/billing/distributors';

export interface IContractDistributor extends IDistributor {
  code: string;
  departmentId: number;
}
