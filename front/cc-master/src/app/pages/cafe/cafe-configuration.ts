import { Injectable } from '@angular/core';

import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from './common/cafe-filters/advanced-filter-form';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';
import { CafeContactConfiguration } from './contacts/cafe-contact-configuration.service';
import { CafeEnvironmentConfiguration } from './environments/cafe-environments.configuration';
import { EstablishmentsConfiguration } from './establishments/establishments.configuration';

@Injectable()
export class CafeConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    ...this.contacts.categories,
    ...this.environments.categories,
    ...this.establishments.categories,
  ];

  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [
    ...this.contacts.advancedFilters,
    ...this.environments.advancedFilters,
    ...this.establishments.advancedFilters,
  ];

  constructor(
    private contacts: CafeContactConfiguration,
    private environments: CafeEnvironmentConfiguration,
    private establishments: EstablishmentsConfiguration,
  ) {
  }
}
