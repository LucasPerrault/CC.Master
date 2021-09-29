import { IComparisonFilterCriterionForm } from './components/comparison-filter-criterion';
import { ILogicalOperator } from './components/logical-operator-select/logical-operator.interface';

export interface IAdvancedFilterForm {
  logicalOperator: ILogicalOperator;
  criterionForms: IComparisonFilterCriterionForm[];
}
