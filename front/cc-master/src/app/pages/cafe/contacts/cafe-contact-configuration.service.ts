import { Injectable } from '@angular/core';

import { ICafeConfiguration } from '../cafe-configuration.interface';
import { IAdvancedFilterConfiguration } from '../common/cafe-filters/advanced-filter-form';
import { ICategory } from '../common/cafe-filters/category-filter/category-select/category.interface';
import { AppContactAdvancedFilterConfiguration } from './app-contacts/advanced-filter/app-contact-advanced-filter.configuration';
import { ContactCategory } from './common/enums/cafe-contacts-category.enum';

@Injectable()
export class CafeContactConfiguration implements ICafeConfiguration {
  public readonly categories: ICategory[] = [
    {
      id: ContactCategory.Contacts,
      name: 'Contacts',
      children: [
        {
          id: ContactCategory.Application,
          name: 'cafe_category_applicationContacts',
        },
      ],
    },
  ];

  public readonly advancedFilters: IAdvancedFilterConfiguration[] = [
    this.appContactsConfiguration,
  ];

  constructor(
    private appContactsConfiguration: AppContactAdvancedFilterConfiguration,
  ) {}
}
