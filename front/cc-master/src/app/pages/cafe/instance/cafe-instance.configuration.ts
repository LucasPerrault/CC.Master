import { Injectable } from '@angular/core';

import { ICafeConfiguration } from '../cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from '../common/cafe-filters/advanced-filter-form';
import { ICategory } from '../common/cafe-filters/category-filter/category-select/category.interface';
import { InstanceCategory } from './enums/instance-category.enum';
import { EnvironmentAdvancedFilterConfiguration } from './advanced-filter/environment-advanced-filter.configuration';

@Injectable()
export class CafeInstanceConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    {
      id: InstanceCategory.Environments,
      name: 'Environnements',
    },
  ];

  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [
    this.environmentConfiguration,
  ];

  constructor(
    private environmentConfiguration: EnvironmentAdvancedFilterConfiguration,
  ) {}
}
