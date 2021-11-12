import { contractFields, IContract } from '@cc/domain/billing/contracts';

import { environmentDetailedFields, IEnvironmentDetailed } from './environment-detailed.interface';
import { IProductSolution, productSolutionFields } from './product-solution.interface';

export const contractEnvironmentFields = [
  contractFields,
  `environment[${environmentDetailedFields}]`,
  'clientId',
  'theoricalStartOn',
  'distributorId',
  `product[${ productSolutionFields }]`,
].join(',');

export interface IContractEnvironmentDetailed extends IContract {
  environment?: IEnvironmentDetailed;
  clientId: number;
  theoricalStartOn: string;
  distributorId: number;
  product: IProductSolution;
}
