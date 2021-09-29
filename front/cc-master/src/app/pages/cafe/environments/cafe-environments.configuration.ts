import { Injectable } from '@angular/core';

import { ICafeConfiguration } from '../cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from '../common/cafe-filters/advanced-filter-form';
import { ICategory } from '../common/cafe-filters/category-filter/category-select/category.interface';
import { EnvironmentAdvancedFilterConfiguration } from './advanced-filter/environment-advanced-filter.configuration';
import { EnvironmentsCategory } from './enums/environments-category.enum';

@Injectable()
export class CafeEnvironmentConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    {
      id: EnvironmentsCategory.Environments,
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
