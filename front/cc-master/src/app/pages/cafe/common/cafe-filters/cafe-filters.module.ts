import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { ClientContactAdvancedFilterModule } from '../../contacts/advanced-filters/client-contacts';
import { AdvancedFiltersModule } from './advanced-filters/advanced-filters.module';
import { CafeFiltersComponent } from './cafe-filters.component';
import { CategoryFilterModule } from './category-filter/category-filter.module';
import { TranslateModule } from '@cc/aspects/translate';


@NgModule({
  declarations: [
    CafeFiltersComponent,
  ],
  exports: [
    CafeFiltersComponent,
  ],
  imports: [
    CommonModule,
    AdvancedFiltersModule,
    ReactiveFormsModule,
    ClientContactAdvancedFilterModule,
    CategoryFilterModule,
    TranslateModule,
  ],
})
export class CafeFiltersModule {
}
