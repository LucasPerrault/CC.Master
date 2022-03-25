import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { IComparisonOperator } from '../components/comparison-filter-criterion/comparison-operator-select/comparison-operator.interface';
import { ComparisonOperator } from '../enums/comparison-operator.enum';

export interface IAdvancedFilterConfiguration {
  criterions: ICriterionConfiguration[];
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
  componentConfigs?: IComponentConfiguration[];
  fields?: FormlyFieldConfig[];
  children?: ICriterionConfiguration[];
}
