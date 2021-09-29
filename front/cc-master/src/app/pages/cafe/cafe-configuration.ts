import { Injectable } from '@angular/core';

import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from './common/cafe-filters/advanced-filter-form';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';

@Injectable()
export class CafeConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [];
  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [];

  constructor() {
  }
}
