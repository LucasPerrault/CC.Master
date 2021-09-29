import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { ComparisonOperator, IAdvancedFilterConfiguration, ICriterionConfiguration } from '../../common/cafe-filters/advanced-filter-form';
import { InstanceCategory } from '../enums/instance-category.enum';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';
import { EnvironmentContactFormlyConfiguration } from './environment-contact-formly.configuration';

@Injectable()
export class EnvironmentAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = InstanceCategory.Environments;
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
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EnvironmentContactFormlyConfiguration,
  ) {}
}
