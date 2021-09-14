import { IProduct, ISolution, productFields, solutionFields } from '@cc/domain/billing/offers';

export const productSolutionFields = `${ productFields },solutions[${ solutionFields }]`;

export interface IProductSolution extends IProduct {
  solutions: ISolution[];
}
