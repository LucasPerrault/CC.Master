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
      key: EnvironmentAdvancedFilterKey.Cluster,
      name: 'Le cluster',
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
      ],
      fields: [this.formlyConfiguration.cluster],
    },
    {
      key: EnvironmentAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: EnvironmentAdvancedFilterKey.Domain,
      name: this.translatePipe.transform('cafe_filters_domain'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.domain],
    },
    {
      key: EnvironmentAdvancedFilterKey.IsActive,
      name: 'Est actif ?',
      operators: [
        { id: ComparisonOperator.TrueOnly, name: 'Oui' },
        { id: ComparisonOperator.FalseOnly, name: 'Non' },
      ],
    },
    {
      key: EnvironmentAdvancedFilterKey.Applications,
      name: this.translatePipe.transform('cafe_filters_applications'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.applications],
    },
    {
      key: EnvironmentAdvancedFilterKey.Distributors,
      name: this.translatePipe.transform('cafe_filters_environment_distributors'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.distributors],
    },
    {
      key: EnvironmentAdvancedFilterKey.Countries,
      name: this.translatePipe.transform('cafe_filters_countries'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.countries],
    },
    {
      key: EnvironmentAdvancedFilterKey.CreatedAt,
      name: this.translatePipe.transform('cafe_filters_environment_createdAt'),
      operators: [
        { id: ComparisonOperator.Since, name: 'après le' },
        { id: ComparisonOperator.Until, name: 'avant le' },
      ],
      fields: [this.formlyConfiguration.createdAt],
    },

  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EnvironmentFormlyConfiguration,
  ) {}
}
