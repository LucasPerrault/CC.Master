import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { CafeConfiguration } from './cafe-configuration';
import { ICafeConfiguration } from './cafe-configuration.interface';
import { IAdvancedFilterForm } from './common/cafe-filters/advanced-filter-form';
import { ContactCategory } from './contacts/common/enums/cafe-contacts-category.enum';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeComponent {
  public cafeFilters: FormControl = new FormControl();
  public configuration: ICafeConfiguration;

  public get category(): ContactCategory {
    return this.cafeFilters.value?.category?.id;
  }

  public get advancedFilterForm(): IAdvancedFilterForm {
    return this.cafeFilters.value?.advancedFilterForm;
  }

  constructor(cafeConfiguration: CafeConfiguration) {
    this.configuration = cafeConfiguration;
  }
}
