import { Injectable } from '@angular/core';

import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from './common/cafe-filters/advanced-filters';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';
import { CafeContactsConfiguration } from './contacts/cafe-contacts-configuration';

@Injectable()
export class CafeConfiguration implements ICafeConfiguration {
  readonly categories: ICategory[] = [
    ...this.contacts.categories,
  ];

  readonly advancedFilters: IAdvancedFilterConfiguration[] = [
    ...this.contacts.advancedFilters,
  ];

  constructor(
    private contacts: CafeContactsConfiguration,
  ) {
  }
}
