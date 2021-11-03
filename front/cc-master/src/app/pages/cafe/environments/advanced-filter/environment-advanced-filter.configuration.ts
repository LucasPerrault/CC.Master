import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../common/cafe-filters/advanced-filter-form';
import { EnvironmentsCategory } from '../enums/environments-category.enum';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';
import { EnvironmentCriterionKey } from './environment-criterion-key.enum';
import { EnvironmentFormlyConfiguration } from './environment-formly-configuration.service';

@Injectable()
export class EnvironmentAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = EnvironmentsCategory.Environments;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: EnvironmentCriterionKey.AppInstances,
      name: this.translatePipe.transform('cafe_filters_environment_applications'),
      operators: [
        { id: ComparisonOperator.ListAreAmong, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.ListNotContains, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
        { id: ComparisonOperator.ListContainsOnly, name: this.translatePipe.transform('cafe_filters_operator_containsOnly') },
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.AppInstances,
          components: [this.formlyConfiguration.applications],
          matchingOperators: [
            ComparisonOperator.ListAreAmong,
            ComparisonOperator.ListNotContains,
            ComparisonOperator.ListContains,
          ],
        },
        {
          key: EnvironmentAdvancedFilterKey.AppInstance,
          components: [this.formlyConfiguration.application],
          matchingOperators: [ComparisonOperator.ListContainsOnly],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.CreatedAt,
      name: this.translatePipe.transform('cafe_filters_environment_createdAt'),
      operators: [
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_since') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_until') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.CreatedAt,
          components: [this.formlyConfiguration.createdAt],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.Countries,
      name: this.translatePipe.transform('cafe_filters_environment_countries'),
      operators: [
        { id: ComparisonOperator.ListAreAmong, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.ListNotContains, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
        { id: ComparisonOperator.ListContainsOnly, name: this.translatePipe.transform('cafe_filters_operator_containsOnly') },
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.Countries,
          components: [this.formlyConfiguration.countries],
          matchingOperators: [
            ComparisonOperator.ListAreAmong,
            ComparisonOperator.ListNotContains,
            ComparisonOperator.ListContains,
          ],
        },
        {
          key: EnvironmentAdvancedFilterKey.Country,
          components: [this.formlyConfiguration.country],
          matchingOperators: [ComparisonOperator.ListContainsOnly],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.Distributors,
      name: this.translatePipe.transform('cafe_filters_environment_distributors'),
      operators: [
        { id: ComparisonOperator.ListAreAmong, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.ListNotContains, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
        { id: ComparisonOperator.ListContainsOnly, name: this.translatePipe.transform('cafe_filters_operator_containsOnly') },
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.Distributors,
          components: [this.formlyConfiguration.distributors],
          matchingOperators: [
            ComparisonOperator.ListAreAmong,
            ComparisonOperator.ListNotContains,
            ComparisonOperator.ListContains,
          ],
        },
        {
          key: EnvironmentAdvancedFilterKey.Distributor,
          components: [this.formlyConfiguration.distributor],
          matchingOperators: [ComparisonOperator.ListContainsOnly],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_environment_subdomain'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.Subdomain,
          components: [this.formlyConfiguration.subdomain],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.Cluster,
      name: this.translatePipe.transform('cafe_filters_environment_cluster'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      componentConfigs: [
        {
          key: EnvironmentAdvancedFilterKey.Cluster,
          components: [this.formlyConfiguration.cluster],
        },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EnvironmentFormlyConfiguration,
  ) {}
}
