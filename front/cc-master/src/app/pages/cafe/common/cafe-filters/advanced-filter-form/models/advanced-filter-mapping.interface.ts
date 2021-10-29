import { IComparisonFilterCriterion, IFilterCriterion } from './advanced-filter.interface';

export const defaultEncapsulation = (criterion: IFilterCriterion) => criterion;
export type IFilterCriterionEncapsulation = (criterion: IFilterCriterion) => IFilterCriterion;

export type IComparisonFilterCriterionEncapsulation = (comparison: IComparisonFilterCriterion) => IFilterCriterion;
