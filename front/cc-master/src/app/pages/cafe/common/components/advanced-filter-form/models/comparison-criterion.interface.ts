import { IFacet } from '../../../models';

export type ComparisonCriterion = IComparisonCriterion | IFacetComparisonCriterion;

export interface IComparisonCriterion {
  key: string;
  name: string;
}

export interface IFacetComparisonCriterion extends IComparisonCriterion {
  facet: IFacet;
}
