import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../common/cafe-filters/advanced-filter-form';
import { EnvironmentsCategory } from '../enums/environments-category.enum';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';
import { EnvironmentFormlyConfiguration } from './environment-formly-configuration.service';

@Injectable()
export class EnvironmentAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = EnvironmentsCategory.Environments;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: EnvironmentAdvancedFilterKey.AppInstances,
      name: this.translatePipe.transform('cafe_filters_environment_applications'),
      operators: [
        { id: ComparisonOperator.ListAreAmong, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.ListNotContains, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
        { id: ComparisonOperator.ListContainsOnly, name: this.translatePipe.transform('cafe_filters_operator_containsOnly') },
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
      ],
      fields: [this.formlyConfiguration.applications],
    },
    {
      key: EnvironmentAdvancedFilterKey.CreatedAt,
      name: this.translatePipe.transform('cafe_filters_environment_createdAt'),
      operators: [
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_since') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_until') },
      ],
      fields: [this.formlyConfiguration.createdAt],
    },
    {
      key: EnvironmentAdvancedFilterKey.Countries,
      name: this.translatePipe.transform('cafe_filters_environment_countries'),
      operators: [
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
      ],
      fields: [this.formlyConfiguration.countries],
    },
    {
      key: EnvironmentAdvancedFilterKey.Distributors,
      name: this.translatePipe.transform('cafe_filters_environment_distributors'),
      operators: [
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
      ],
      fields: [this.formlyConfiguration.distributors],
    },
    {
      key: EnvironmentAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_environment_subdomain'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      fields: [this.formlyConfiguration.subdomain],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EnvironmentFormlyConfiguration,
  ) {}
}
