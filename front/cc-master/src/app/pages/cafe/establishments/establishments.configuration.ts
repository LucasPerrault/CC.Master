import { Injectable } from '@angular/core';

import { ICafeConfiguration } from '../cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from '../common/cafe-filters/advanced-filter-form';
import { ICategory } from '../common/cafe-filters/category-filter/category-select/category.interface';
import { EstablishmentsCategory } from './establishments-category.enum';

@Injectable()
export class EstablishmentsConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    {
      id: EstablishmentsCategory.Establishments,
      name: 'cafe_category_establishments',
    },
  ];

  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [];

  constructor() {}
}
