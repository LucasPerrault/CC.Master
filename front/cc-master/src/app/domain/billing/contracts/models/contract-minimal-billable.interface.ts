import { IDistributor } from '@cc/domain/billing/distributors';

export interface IContractMinimalBillable {
  theoreticalMonthRebate: number;
  productId: number;
  distributor: IDistributor;
}
