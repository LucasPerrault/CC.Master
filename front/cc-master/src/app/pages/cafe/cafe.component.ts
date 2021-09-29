import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { CafeConfiguration } from './cafe-configuration';
import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterForm } from './common/cafe-filters/advanced-filter-form';
import { ContactCategory } from './contacts/common/enums/cafe-contacts-category.enum';
import { EnvironmentsCategory } from './environments/enums/environments-category.enum';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeComponent {
  public cafeFilters: FormControl = new FormControl();
  public configuration: ICafeConfiguration;

  public get category(): ContactCategory | EnvironmentsCategory {
    return this.cafeFilters.value?.category?.id;
  }

  public get advancedFilterForm(): IAdvancedFilterForm {
    return this.cafeFilters.value?.advancedFilterForm;
  }

  public get isEnvironmentCategory(): boolean {
    return this.category === EnvironmentsCategory.Environments;
  }


  public get isContactCategory(): boolean {
    return !!this.contactCategories.find(c => c === this.category);
  }

  private readonly contactCategories = [
    ContactCategory.Client,
    ContactCategory.Specialized,
    ContactCategory.Application,
    ContactCategory.All,
  ];

  constructor(cafeConfiguration: CafeConfiguration) {
    this.configuration = cafeConfiguration;
  }
}
