import { FilterCriterion, IComparisonFilterCriterion } from './advanced-filter.interface';

export const defaultEncapsulation = (criterion: FilterCriterion) => criterion;
export type IFilterCriterionEncapsulation = (criterion: FilterCriterion) => FilterCriterion;

export type IComparisonFilterCriterionEncapsulation = (comparison: IComparisonFilterCriterion) => FilterCriterion;
