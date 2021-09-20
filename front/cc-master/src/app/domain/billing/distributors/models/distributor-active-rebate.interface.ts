import { distributorFields, IDistributor } from './distributor.interface';
import { IRebate, rebateFields } from './rebate.interface';

export const distributorActiveRebateFields = `${ distributorFields },activeRebates[${ rebateFields }]`;
export interface IDistributorActiveRebate extends IDistributor {
  activeRebates: IRebate[];
}


