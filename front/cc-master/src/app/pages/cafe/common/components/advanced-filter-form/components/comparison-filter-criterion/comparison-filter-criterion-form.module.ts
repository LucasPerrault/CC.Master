import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

import { ComparisonCriterionSelectModule } from './comparison-criterion-select/comparison-criterion-select.module';
import { ComparisonFilterCriterionFormComponent } from './comparison-filter-criterion-form.component';
import { ComparisonOperatorSelectModule } from './comparison-operator-select/comparison-operator-select.module';
import { ComparisonValueSelectModule } from './comparison-value-select/comparison-value-select.module';

@NgModule({
  declarations: [
    ComparisonFilterCriterionFormComponent,
  ],
  exports: [
    ComparisonFilterCriterionFormComponent,
  ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        ComparisonCriterionSelectModule,
        ComparisonOperatorSelectModule,
        ComparisonValueSelectModule,
    ],
})
export class ComparisonFilterCriterionFormModule { }
