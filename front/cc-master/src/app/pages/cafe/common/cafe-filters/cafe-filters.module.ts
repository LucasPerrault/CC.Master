import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { AdvancedFilterFormModule } from './advanced-filter-form/advanced-filter-form.module';
import { CafeFiltersComponent } from './cafe-filters.component';
import { CategoryFilterModule } from './category-filter/category-filter.module';


@NgModule({
  declarations: [
    CafeFiltersComponent,
  ],
  exports: [
    CafeFiltersComponent,
  ],
  imports: [
    CommonModule,
    AdvancedFilterFormModule,
    ReactiveFormsModule,
    CategoryFilterModule,
    TranslateModule,
  ],
})
export class CafeFiltersModule {
}
