import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { CategoryFilterComponent } from './category-filter.component';
import { CategorySelectModule } from './category-select/category-select.module';



@NgModule({
  declarations: [
    CategoryFilterComponent,
  ],
  exports: [
    CategoryFilterComponent,
  ],
  imports: [
    CommonModule,
    CategorySelectModule,
    TranslateModule,
    ReactiveFormsModule,
  ],
})
export class CategoryFilterModule { }
