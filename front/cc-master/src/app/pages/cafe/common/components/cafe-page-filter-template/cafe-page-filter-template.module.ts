import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { AdvancedFilterFormModule } from '../../cafe-filters/advanced-filter-form/advanced-filter-form.module';
import { CategorySelectModule } from '../../cafe-filters/category-filter/category-select/category-select.module';
import { CafePageFilterTemplateComponent } from './cafe-page-filter-template.component';

@NgModule({
  declarations: [CafePageFilterTemplateComponent],
  imports: [
    CommonModule,
    CategorySelectModule,
    TranslateModule,
    ReactiveFormsModule,
    AdvancedFilterFormModule,
  ],
  exports: [CafePageFilterTemplateComponent],
})
export class CafePageFilterTemplateModule {
}
