import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { AdvancedCriterionFormComponent } from './advanced-criterion-form.component';
import { ComparisonCriterionSelectModule } from './comparison-criterion-select/comparison-criterion-select.module';
import { ComparisonFilterCriterionFormModule } from './comparison-filter-criterion-form/comparison-filter-criterion-form.module';

@NgModule({
  declarations: [
    AdvancedCriterionFormComponent,
  ],
  exports: [
    AdvancedCriterionFormComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ComparisonCriterionSelectModule,
    ComparisonFilterCriterionFormModule,
  ],
})
export class AdvancedCriterionFormModule {}
