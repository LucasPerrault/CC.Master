import { Injectable } from '@angular/core';

import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from './common/cafe-filters/advanced-filter-form';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';
import { CafeContactConfiguration } from './contacts/cafe-contact-configuration.service';

@Injectable()
export class CafeConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    ...this.contacts.categories,
  ];

  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [
    ...this.contacts.advancedFilters,
  ];

  constructor(private contacts: CafeContactConfiguration) {
  }
}
