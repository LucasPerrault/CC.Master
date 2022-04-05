import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LuFormlyModule } from '@lucca-front/ng/formly';
import { FormlyModule } from '@ngx-formly/core';

import { CCFormlyModule } from '../../../../../forms/formly';
import { ComparisonCriterionSelectComponent } from './comparison-criterion-select.component';

@NgModule({
  declarations: [
    ComparisonCriterionSelectComponent,
  ],
  exports: [
    ComparisonCriterionSelectComponent,
  ],
  imports: [
    CommonModule,
    LuFormlyModule,
    CCFormlyModule,
    FormlyModule.forRoot({ extras: { immutable: true } }),
    ReactiveFormsModule,
  ],
})
export class ComparisonCriterionSelectModule {
}
