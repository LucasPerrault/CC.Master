import { contractFields, IContract } from '@cc/domain/billing/contracts';

import { IProductSolution, productSolutionFields } from './product-solution.interface';

export const previousContractEnvironmentFields = `${ contractFields },endOn,distributorId,product[${ productSolutionFields }]`;

export interface IPreviousContractEnvironment extends IContract {
  endOn: string;
  distributorId: string;
  product: IProductSolution;
}
