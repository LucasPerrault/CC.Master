import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { ComparisonOperator, IAdvancedFilterConfiguration, ICriterionConfiguration } from '../../common/cafe-filters/advanced-filter-form';
import { EnvironmentsCategory } from '../enums/environments-category.enum';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';
import { EnvironmentFormlyConfiguration } from './environment-formly-configuration.service';

@Injectable()
export class EnvironmentAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = EnvironmentsCategory.Environments;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: EnvironmentAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: EnvironmentAdvancedFilterKey.Domain,
      name: this.translatePipe.transform('cafe_filters_domain'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.domain],
    },
    {
      key: EnvironmentAdvancedFilterKey.IsActive,
      name: this.translatePipe.transform('cafe_filters_isActive'),
      operators: [ComparisonOperator.TrueOnly, ComparisonOperator.FalseOnly],
    },
    {
      key: EnvironmentAdvancedFilterKey.Applications,
      name: this.translatePipe.transform('cafe_filters_applications'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.applications],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EnvironmentFormlyConfiguration,
  ) {}
}
