import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { IComparisonFilterCriterion, IFilterCriterion } from './advanced-filter.interface';

export interface IAdvancedFilterConfiguration {
  categoryId: string;
  criterions: ICriterionConfiguration[];
}

export interface ICriterionConfiguration {
  key: string;
  name: string;
  operators: ComparisonOperator[];
  fields: FormlyFieldConfig[];
  mapping: ICriterionMapping;
}

export interface ICriterionMapping {
  toApiQueries: (o: any) => string[];
  toFilterCriterion: (c: IComparisonFilterCriterion) => IFilterCriterion;
}
