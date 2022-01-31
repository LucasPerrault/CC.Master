import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { CategorySelectModule } from '../../cafe-filters/category-filter/category-select/category-select.module';
import { CafePageTemplateComponent } from './cafe-page-template.component';

@NgModule({
  declarations: [CafePageTemplateComponent],
  exports: [CafePageTemplateComponent],
  imports: [
    CommonModule,
    TranslateModule,
    CategorySelectModule,
    ReactiveFormsModule,
  ],
})
export class CafePageTemplateModule { }
