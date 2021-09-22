import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoryFilterComponent } from './category-filter.component';
import { CategorySelectModule } from './category-select/category-select.module';
import { TranslateModule } from '@cc/aspects/translate';
import { ReactiveFormsModule } from '@angular/forms';



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
