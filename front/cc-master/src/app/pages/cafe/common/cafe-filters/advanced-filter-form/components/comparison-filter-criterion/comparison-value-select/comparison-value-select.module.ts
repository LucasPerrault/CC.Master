import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { LuFormlyModule } from '@lucca-front/ng/formly';
import { FormlyModule } from '@ngx-formly/core';

import { CCFormlyModule } from '../../../../../forms/formly';
import { ComparisonValueSelectComponent } from './comparison-value-select.component';

@NgModule({
  declarations: [
    ComparisonValueSelectComponent,
  ],
  exports: [
    ComparisonValueSelectComponent,
  ],
  imports: [
    CommonModule,
    LuFormlyModule,
    CCFormlyModule,
    FormlyModule.forRoot({ extras: { immutable: true } }),
    ReactiveFormsModule,
  ],
})
export class ComparisonValueSelectModule {
}
