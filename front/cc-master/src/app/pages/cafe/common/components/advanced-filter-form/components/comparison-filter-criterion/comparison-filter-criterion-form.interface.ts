import { IComparisonCriterion } from '../../models/comparison-criterion.interface';
import { IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';
import { IComparisonValue } from './comparison-value-select/comparison-value.interface';

export interface IComparisonFilterCriterionForm {
  criterion: IComparisonCriterion;
  operator: IComparisonOperator;
  values: IComparisonValue;
}
