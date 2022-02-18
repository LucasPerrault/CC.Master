import { IComparisonFilterCriterion, IFilterCriterion, IListFilterCriterion } from './advanced-filter.interface';

export const defaultEncapsulation = (criterion: IFilterCriterion | IListFilterCriterion) => criterion;
export type IFilterCriterionEncapsulation = (criterion: IFilterCriterion | IListFilterCriterion) => IFilterCriterion | IListFilterCriterion;

export type IComparisonFilterCriterionEncapsulation = (comparison: IComparisonFilterCriterion) => IFilterCriterion | IListFilterCriterion;
