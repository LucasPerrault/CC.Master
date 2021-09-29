import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ComparisonOperator } from '../enums/comparison-operator.enum';

export interface IAdvancedFilterConfiguration {
  categoryId: string;
  criterions: ICriterionConfiguration[];
}

export interface ICriterionConfiguration {
  key: string;
  name: string;
  operators?: ComparisonOperator[];
  fields?: FormlyFieldConfig[];
  children?: ICriterionConfiguration[];
}
