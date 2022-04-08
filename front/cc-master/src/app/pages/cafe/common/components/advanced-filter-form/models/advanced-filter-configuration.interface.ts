import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { IComparisonOperator } from '../components/advanced-criterion-form';
import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { IComparisonFilterCriterionValue } from './advanced-filter.interface';
import { ComparisonCriterion } from './comparison-criterion.interface';

export interface IAdvancedFilterConfiguration {
  criterions: ICriterionConfiguration[];
  criterionFormlyFieldConfigs?: FormlyFieldConfig[];
}

export interface IComponentConfiguration {
  /**
   * @description This key is used to map the selected comparison values with its operators
   * and in the api-mapping service.
   */
  key: string;
  components?: FormlyFieldConfig[];
  matchingOperators?: ComparisonOperator[];
  defaultValue?: IComparisonFilterCriterionValue;
}

export interface ICriterionConfiguration {
  /**
   * @description This key is used to map the selected comparison criterion with its configuration
   * and in the api-mapping service.
   */
  key: string;
  name: string;
  /**
   * @description Default selected operator. By default, the first of the list is selected.
   */
  defaultOperator?: IComparisonOperator;
  /**
   * @description All the operators linked with the criterion.
   */
  operators?: IComparisonOperator[];
  /**
   * @description Child of criterion is another criterion.
   */
  children?: ICriterionConfiguration[];
  childrenFormlyFieldConfigs?: FormlyFieldConfig[];

  /**
   * @description The criterion component configuration use in the comparison criterion selection
   * @param criterion
   */
  componentConfigs?(criterion?: ComparisonCriterion): IComponentConfiguration[];
}
