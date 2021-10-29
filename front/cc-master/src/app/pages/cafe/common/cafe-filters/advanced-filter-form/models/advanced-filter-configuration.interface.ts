import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { IComparisonOperator } from '../components/comparison-filter-criterion/comparison-operator-select/comparison-operator.interface';

export interface IAdvancedFilterConfiguration {
  categoryId: string;
  criterions: ICriterionConfiguration[];
}

export interface ICriterionConfiguration {
  key: string;
  name: string;
  operators?: IComparisonOperator[];
  fields?: FormlyFieldConfig[];
  children?: ICriterionConfiguration[];
}
