import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { IComparisonOperator } from '../components/comparison-filter-criterion/comparison-operator-select/comparison-operator.interface';
import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { ComparisonCriterion } from './comparison-criterion.interface';

export interface IAdvancedFilterConfiguration {
  criterions: ICriterionConfiguration[];
  criterionFormlyFieldConfigs?: FormlyFieldConfig[];
}

export interface IComponentConfiguration {
  key: string;
  components: FormlyFieldConfig[];
  matchingOperators?: ComparisonOperator[];
}

export interface ICriterionConfiguration {
  key: string;
  name: string;
  operators?: IComparisonOperator[];
  fields?: FormlyFieldConfig[];
  children?: ICriterionConfiguration[];
  childrenFormlyFieldConfigs?: FormlyFieldConfig[];
  componentConfigs?(criterion?: ComparisonCriterion): IComponentConfiguration[];
}
